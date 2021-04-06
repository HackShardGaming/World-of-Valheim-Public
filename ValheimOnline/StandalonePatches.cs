using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using ValheimOnline.Console;

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

#if client_cli
        // Patches assembly_valheim::Version::GetVersionString
        // Links in our version detail to override games original one to maintain compatibility
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Chat), "InputText")]
        private static void Chat__InputText(ref Chat __instance)
        {
            var text = __instance.m_input.text;
            // Parse client or server commands.
            Runner console = new Runner();
            console.RunCommand(text, false);
        }
#endif

        // Patches assembly_valheim::FejdStartup::Update
        // Note: Main class for the game
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FejdStartup), "Update")]
        private static void FejdStartup__Update(GameObject ___m_startGamePanel, Button ___m_worldStart)
        {
            if (___m_startGamePanel.activeInHierarchy)
            {
                // Should we allow single player? Yes or no.
                // Config must have both AllowSinglePlayer and ExportCharacter set to true in order to continue.
                if (ValheimOnline.AllowSinglePlayer.Value && ValheimOnline.ExportCharacter.Value)
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
                ZoneHandler.ZoneTypes ztype;
                bool changed;
                bool zonedDetected = ZoneHandler.Detect(Player.m_localPlayer.transform.position, out changed, out zone, out ztype);
                if (changed)
                {
                    if (zonedDetected)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"You have now entered {zone.Name}",
                            0, null);

                    }
                    else
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"You have now entered the wilderness",
                            0, null);
                        
                    }

                    // Zones are now being enforced?
                    if (Client.EnforceZones)
                    {
                        // Update the client settings based on zone type

                        // PVP settings:
                        Client.PVPEnforced = ztype.PVPEnforce;
                        if (ztype.PVPEnforce)
                            Client.PVPMode = ztype.PVP;

                        // Position settings:
                        Client.PositionEnforce = ztype.PositionEnforce;
                        if (ztype.PositionEnforce)
                            Client.ShowPosition = ztype.ShowPosition;

                        // Run the updated settings for the Clients
                        Player.m_localPlayer.SetPVP(Client.PVPMode);
                        ZNet.instance.SetPublicReferencePosition(Client.ShowPosition);

                        // Other settings are scattered among the wind to other functions
                        // (Use Client class for the current state)
                    }
#if DEBUG
                    ZoneHandler._debug(ztype);
                    Client._debug();
#endif
                }
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
            Debug.Log($"Server Zone Enforced: {Client.EnforceZones}");
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
#if DEBUG
            ZoneHandler._debug();
#endif


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
            if (StandalonePatches.m_quitting || StandalonePatches.m_logging)
            {
                Debug.Log("We are logging out with the Logout or Quit button! Skipping this patch!");
                return true;
            }
            StandalonePatches.m_quitting = true;
            
            if (ZNet.instance.IsServer())
            {
                Util.ServerShutdown();
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

        public static bool m_quitting;

        public static bool m_logging;
    }
}
