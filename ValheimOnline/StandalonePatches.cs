using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace ValheimOnline
{

    [HarmonyPatch]
    public static class StandalonePatches
    {

#if CHANGELOG_EN
        // Patches assembly_valheim::ChangeLog::Start
        // Attaches our mod details to the games changelog
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChangeLog), "Start")]
        private static void ChangeLog__Start(ref TextAsset ___m_changeLog)
        {
            string str = $"{ModInfo.GetBuildDate()} {ModInfo.Name} v{ModInfo.Version}\n" + "* Server-side characters: when the client connects to the server, it is provided with a character to use by the server. The server maintains ownership of the client's character.\n* Safe Zones: In Valheim Online, most of the world is known as \"wilderness\". In the wilderness, PvP is forced on and map marker sharing off. Server admins can specify one or more safe zones which have PvP forced off.\n* Damn That Crow: The \"I have arrived\" shout does not belong in a persistent world. It has been removed.\n\n";
            ___m_changeLog = new TextAsset(str + ___m_changeLog.text);
        }
#endif

        // Patches assembly_valheim::Version::GetVersionString
        // Links in our version detail to override games original one to maintain compatibility
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Version), "GetVersionString")]
        private static void Version__GetVersionString(ref string __result)
        {
#if DEBUG
            __result = $"{__result} ({ModInfo.Name} v{ModInfo.Version}-Dev)";
#else
            __result = $"{__result} ({ModInfo.Name} v{ModInfo.Version})";
#endif
        }

        // Patches assembly_valheim::FejdStartup::Update
        // Note: Main class for the game
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FejdStartup), "Update")]
        private static void FejdStartup__Update(GameObject ___m_startGamePanel, Button ___m_worldStart)
        {
            if (___m_startGamePanel.activeInHierarchy)
            {
                // Should we allow single player? Yes or no.
                // Config must have both AllowSinglePlayer and AllowCharacterSave set to true in order to continue.
                if (ValheimOnline.AllowSinglePlayer.Value && ValheimOnline.AllowCharacterSave.Value)
                {
                    GameObject gameObject = GameObject.Find("Start");
                    if (gameObject != null)
                    {
                        Text componentInChildren = gameObject.GetComponentInChildren<Text>();
                        if (componentInChildren != null)
                        {
                            componentInChildren.text = "Start (WARNING MANY CONSOLE ERRORS)";
                        }
                    }
                    ___m_worldStart.interactable = true;
                }
                else
                {
                    GameObject gameObject = GameObject.Find("Start");
                    if (gameObject != null)
                    {
                        Text componentInChildren = gameObject.GetComponentInChildren<Text>();
                        if (componentInChildren != null)
                        {
                            componentInChildren.text = "Disabled in Valheim Online";
                        }
                    }
                    ___m_worldStart.interactable = false;
                }
            }
        }

        //
        // This is the bread and butter of maintaining the user PVP state
        //
        // This class will constantly check for any updates in position and set the client PVP state accordingly.
        //
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game), "Update")]
        private static void Game__Update()
        {
            // Are we the Server?
            if (ZNet.instance.IsServer())
            {
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                using (List<ServerState.ConnectionData>.Enumerator enumerator = ServerState.Connections.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ServerState.ConnectionData connectionData = enumerator.Current;
                        if (realtimeSinceStartup - connectionData.last_save_time >= (float)ValheimOnline.ServerSaveInterval.Value)
                        {
                            connectionData.rpc.Invoke("ServerVaultUpdate", new object[]
                            {
                                new ZPackage()
                            });
                            connectionData.last_save_time = realtimeSinceStartup;
                        }
                    }
                    return;
                }
            }

            if (Player.m_localPlayer)
            {
                // Goes through the safezone list and check if we are in the zone. If so we are safe and got nothing to worry about.
                // 
                //
                // Goes through the zones and setup the necessary enforcements.

                ZoneHandler.Zone zone;
                bool changed;
                bool zonedDetected = ZoneHandler.Detect(Player.m_localPlayer.transform.position, out changed, out zone);
                if (changed)
                {
                    if (zonedDetected)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"You have now entered {zone.Name}",
                            0, null);
                        if (Client.PVPEnforced == true)
                        {
                            Client.PVPMode = zone.pvp;
                        }
                    }
                    else
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"You have now entered the wilderness",
                            0, null);
                        if (Client.PVPEnforced == true)
                        {
                            Client.PVPMode = Client.PVPisEnabled;
                        }
                    }

                    // Process the state of player based on the flag.
                    if (Client.PVPEnforced == true)
                    {
                        Player.m_localPlayer.SetPVP(Client.PVPMode);
                    }

                    // Tells the world where we are in reference
                    if (Client.PositionEnforced == true)
                    {
                        ZNet.instance.SetPublicReferencePosition(Client.PVPSharePosition);
                    }
                }
                /*
                ServerState.SafeZone safeZone;
                bool flag = Util.PointInSafeZone(Player.m_localPlayer.transform.position, out safeZone);

                ServerState.BattleZone battleZone;
                bool flag2 = Util.PointInBattleZone(Player.m_localPlayer.transform.position, out battleZone);
                if (Client.PVPEnforced == false)
                {
                    if (flag && !Client.InSafeZone)
                    {
#if DEBUG
                        Client._debug();
#endif
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"You have now entered safe zone {safeZone.name}", 0, null);
                        Client.InSafeZone = true;

                    }
                    else if (flag2 && !Client.InBattleZone)
                    {
#if DEBUG
                        Client._debug();
#endif
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"You have now entered battle zone {battleZone.name}", 0, null);
                        Client.InBattleZone = true;
                    }
                    else if (!flag && !flag2 && ( Client.InSafeZone || Client.InBattleZone) )
                    {
#if DEBUG
                        Client._debug();
#endif
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You are now in the wilderness", 0, null);
                        Client.InSafeZone = false;
                        Client.InBattleZone = false;
                    }
                }
                if (Client.PVPEnforced == true)
                {
                    if (Client.PVPisEnabled == true)
                    {
                        if (flag && !Client.InSafeZone)
                        {
#if DEBUG
                            Client._debug();
#endif
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"You have now entered safe zone {safeZone.name}", 0, null);
                            Client.InSafeZone = true;
                            Client.PVPMode = false;

                        }
                        else if (!flag && Client.InSafeZone)
                        {
#if DEBUG
                            Client._debug();
#endif
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You are now in the wilderness", 0, null);

                            Client.InSafeZone = false;
                            Client.PVPMode = true;
                        }
                    }
                    else if (Client.PVPisEnabled == false)
                    {
                        if (flag2 && !Client.InBattleZone)
                        {
#if DEBUG
                            Client._debug();
#endif
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"You have now entered battle zone {battleZone.name}", 0, null);
                            Client.InBattleZone = true;
                            Client.PVPMode = true;
                        }
                        else if (!flag2 && Client.InBattleZone)
                        {
#if DEBUG
                            Client._debug();
#endif
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You are now in the wilderness", 0, null);

                            Client.InBattleZone = false;
                            Client.PVPMode = false;
                        }
                    }

                    // Process the state of player based on the flag.
                    Player.m_localPlayer.SetPVP(Client.PVPMode);
                    // Tells the world where we are in reference
                    if (Client.PositionEnforced == true)
                    {
                        ZNet.instance.SetPublicReferencePosition(Client.PVPSharePosition);
                    }
                }

                */
            }

            if (ServerState.ClientMayDisconnect)
            {
                Debug.Log($"Deferred log m_quitting: {StandalonePatches.m_quitting} m_logging: {StandalonePatches.m_logging}");
                if (StandalonePatches.m_quitting)
                {
                    Menu.instance.OnQuitYes();
                }
                else
                {
                    Menu.instance.OnLogoutYes();
                }
                ServerState.Connections.Clear();
                ServerState.ClientMayDisconnect = false;
                StandalonePatches.m_quitting = false;
                StandalonePatches.m_logging = false;
            }
        }

        // Patch ZNet::OnNewConnection
        // This is where a client setup a connection to the server (vice versa)
        // Put any RPC register here to sync between server/client.
        //
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ZNet), "OnNewConnection")]
        private static void ZNet__OnNewConnection(ZNet __instance, ZNetPeer peer)
        {
            Debug.Log($"Server PVP Enforce: {Client.PVPEnforced}");
            if (!__instance.IsServer())
            {
                // Client special RPC calls
                peer.m_rpc.Register<ZPackage>("ServerVaultData", new Action<ZRpc, ZPackage>(RPC.ServerVaultData));
                peer.m_rpc.Register<ZPackage>("ZoneHandler", new Action<ZRpc, ZPackage>(ZoneHandler.RPC));
                peer.m_rpc.Register<ZPackage>("SafeZones", new Action<ZRpc, ZPackage>(RPC.SafeZones));
                peer.m_rpc.Register<ZPackage>("BattleZones", new Action<ZRpc, ZPackage>(RPC.BattleZones));
                peer.m_rpc.Register<ZPackage>("Client", new Action<ZRpc, ZPackage>(Client.RPC));

                // Reset the state of the server if we DC and reconnect.
                ServerState.ClientMayDisconnect = false;
                StandalonePatches.m_quitting = false;
                StandalonePatches.m_logging = false;
            }
            peer.m_rpc.Register<ZPackage>("ServerVaultUpdate", new Action<ZRpc, ZPackage>(RPC.ServerVaultUpdate));
            peer.m_rpc.Register<ZPackage>("ServerQuit", new Action<ZRpc, ZPackage>(RPC.ServerQuit));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "OnDeath")]
        private static void Player__OnDeath(Player __instance)
        {
            if (ZNet.instance.IsServer())
            {
                return;
            }
            if (__instance == Player.m_localPlayer)
            {
                Debug.Log("Char update (death)");
                PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                playerProfile.ClearLoguoutPoint();
                Util.GetServer().rpc.Invoke("ServerVaultUpdate", new object[]
                {
                    Util.Compress(playerProfile.Serialize(__instance, false))
                });
            }
        }

        // Patch ZNet::SendPeerInfo
        // During connection, use to send info to the peer.
        // Great point to send to client.

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ZNet), "SendPeerInfo")]
        private static void ZNet__SendPeerInfo(ZNet __instance, ZRpc rpc)
        {
            // Run away clients, we don't want you here!?!?
            if (!__instance.IsServer())
            {
                return;
            }
            Debug.Log("S2C ServerVaultData");
            rpc.Invoke("ServerVaultData", new object[] {
                Util.Compress(Util.LoadOrMakeCharacter(rpc.GetSocket().GetHostName()))
            });

            Debug.Log("S2C ZoneHandler (SendPeerInfo)");
            ZoneHandler._debug();
            rpc.Invoke("ZoneHandler", new object[] {
                ZoneHandler.Serialize()
            });

            Debug.Log("S2C SafeZones");
            rpc.Invoke("SafeZones", new object[] {
                ServerState.SafeZones.Serialize()
            });

            Debug.Log("S2C BattleZone");
            rpc.Invoke("BattleZones", new object[] {
                ServerState.BattleZones.Serialize()
            });

            Debug.Log("S2C ClientState (SendPeerInfo)");
#if DEBUG
            Client._debug();
#endif
            rpc.Invoke("Client", new object[] {
                Client.Serialize()
            });


            ServerState.Connections.Add(new ServerState.ConnectionData
            {
                rpc = rpc
            });
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu), "OnQuitYes")]
        private static bool Menu__OnQuitYes()
        {
            Debug.Log("Quit Detected.");
            if (StandalonePatches.m_quitting)
            {
                return true;
            }
            StandalonePatches.m_quitting = true;
            Debug.Assert(!ZNet.instance.IsServer());
            Debug.Log("Quitting: sending ServerQuit and waiting.");
            Util.GetServer().rpc.Invoke("ServerQuit", new object[]
            {
                Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
            });
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Game), "Shutdown")]
        private static bool Game__Shutdown()
        {
            Debug.Log("Shutdown Detected.");
            if (StandalonePatches.m_quitting)
            {
                return true;
            }
            StandalonePatches.m_quitting = true;

            if (ZNet.instance.IsServer())
            {
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                using (List<ServerState.ConnectionData>.Enumerator enumerator = ServerState.Connections.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ServerState.ConnectionData connectionData = enumerator.Current;
                        connectionData.rpc.Invoke("ServerVaultUpdate", new object[]
                        {
                            new ZPackage()
                        });
                        connectionData.last_save_time = realtimeSinceStartup;
                    }
                    Debug.Log("Sending Requests to clients to save!");
                }
                return false;
            }
            else
            {
                Debug.Assert(!ZNet.instance.IsServer());
                Debug.Log("Quitting: sending ServerQuit and waiting.");
                Util.GetServer().rpc.Invoke("ServerQuit", new object[]
                {
                    Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
                });
                return false;
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu), "OnLogoutYes")]
        private static bool Menu__OnLogoutYes()
        {
            Debug.Log("Logout Detected.");
            if (StandalonePatches.m_logging)
            {
                return true;
            }
            StandalonePatches.m_logging = true;
            Debug.Assert(!ZNet.instance.IsServer());
            Debug.Log("Logging out: sending ServerQuit and waiting.");
            Util.GetServer().rpc.Invoke("ServerQuit", new object[]
            {
                Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
            });
            return false;
        }

        private static bool m_quitting;

        private static bool m_logging;
    }
}
