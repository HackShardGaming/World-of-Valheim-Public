using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace WorldofValheimServerSideCharacters
{

    [HarmonyPatch]
    public static class StandalonePatches
    {


        // Patches assembly_valheim::Version::GetVersionString
        // Links in our version detail to override games original one to maintain compatibility
        [HarmonyPatch(typeof(Version), "GetVersionString")]
        public static class Version_GetVersionString_Patch
        {
            [HarmonyBefore(new string[] { "mod.valheim_plus" })]
            private static void Postfix(ref string __result)
            {
#if DEBUG
                __result = $"{__result} ({ModInfo.Name} v{ModInfo.Version}-Dev)";
#else

                __result = $"{__result} ({ModInfo.Name} v{ModInfo.Version})";
                //Debug.Log($"Version Generated: {__result}");
#endif
            }
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
        [HarmonyPatch(typeof(Game), "Start")]
        public static class GameStartPatch
        {
            private static void Prefix()
            {
                Debug.Log("New Connection! Reseting Connection Count to 0");
                ServerState.ConnectionCount = 0;
                ZRoutedRpc.instance.Register("ShutdownServer", new Action<long, ZPackage>(RPC.ShutdownServer)); // Server Shutdown Registering
                ZRoutedRpc.instance.Register("SaveAll", new Action<long, ZPackage>(RPC.SaveAll)); // Save all online users
                ZRoutedRpc.instance.Register("ReloadDefault", new Action<long, ZPackage>(RPC.ReloadDefault)); // Save all online users
            }
        }

        // Patches assembly_valheim::FejdStartup::Update
        // Note: Main class for the game
        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FejdStartup), "Update")]
        private static void FejdStartup__Update(GameObject ___m_startGamePanel, Button ___m_worldStart)
        {
            if (___m_startGamePanel.activeInHierarchy)
            {
                // Should we allow single player? Yes or no.
                // Config must have both AllowSinglePlayer and ExportCharacter set to true in order to continue.
                if (WorldofValheimServerSideCharacters.AllowSinglePlayer.Value && WorldofValheimServerSideCharacters.ExportCharacter.Value)
                {
                    GameObject gameObject = GameObject.Find("Start");
                    if (gameObject != null)
                    {
                        Text componentInChildren = gameObject.GetComponentInChildren<Text>();
                        if (componentInChildren != null)
                        {
                            componentInChildren.text = "Start";
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
                            componentInChildren.text = "Disabled in World of Valheim - SSC";
                        }
                    }
                    ___m_worldStart.interactable = false;
                }
            }
        }
        */

        //
        // This is the class that controls allowing logout or quit
        // For the server side it will set the timeframe inbetween requesting client updates
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
                        if (realtimeSinceStartup - connectionData.LastTimeSaved >= (float)WorldofValheimServerSideCharacters.SaveInterval.Value)
                        {
                            connectionData.rpc.Invoke("CharacterUpdate", new object[]
                            {
                                new ZPackage()
                            });
                            connectionData.LastTimeSaved = realtimeSinceStartup;
                        }
                    }
                    return;
                }
            }
            // Ok we are the client! Can we DC?
            if (ServerState.ClientCanDC)
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
                ServerState.ClientCanDC = false;
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
            Debug.Log("Checking connection?");
            // Are we the Client?
            if (!__instance.IsServer())
            {
                // Good here is the client specifics.
                peer.m_rpc.Register<ZPackage>("CharacterData", new Action<ZRpc, ZPackage>(RPC.CharacterData));

                // Reset the state of the server if we DC and reconnect.
                ServerState.ClientCanDC = false;
                StandalonePatches.m_quitting = false;
                StandalonePatches.m_logging = false;
            }
            // Ok now for the rest. Including if we are the server.
            peer.m_rpc.Register<ZPackage>("CharacterUpdate", new Action<ZRpc, ZPackage>(RPC.CharacterUpdate));
            peer.m_rpc.Register<ZPackage>("ExitServer", new Action<ZRpc, ZPackage>(RPC.ExitServer));
        }

        // Patch Player::OnDeath
        // This is the patch that resets the character if they died.
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
                Debug.Log("Updating Character since we died!");
                PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                playerProfile.ClearLoguoutPoint();
                Util.GetServer().rpc.Invoke("CharacterUpdate", new object[]
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
            // Are we the client? Then get out!
            if (!__instance.IsServer())
            {
                return;
            }
            // Ok now that that's done. Lets gogogo Servers!
            Game.instance.StartCoroutine(Util.CharacterData(rpc));
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
            Debug.Log($"Connection Count {ServerState.Connections.Count}");
            ServerState.ConnectionCount = ServerState.Connections.Count;
            if (ServerState.Connections.Count > 0)
            {
                Debug.Log("Quitting: sending ExitServer and waiting.");
                Util.GetServer().rpc.Invoke("ExitServer", new object[]
                {
                Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
                });
                return false;
            }
            else
            {
                return true;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Game), "Shutdown")]
        private static bool Game__Shutdown()
        {
            Debug.Log("Shutdown Detected.");
            if (StandalonePatches.m_quitting || StandalonePatches.m_logging)
            {
                return true;
            }
            StandalonePatches.m_quitting = true;

            if (ZNet.instance.IsServer())
            {
                Game.instance.StartCoroutine(Util.ShutdownServer());
                return false;
            }
            else
            {
                Debug.Assert(!ZNet.instance.IsServer());
                Debug.Log($"Connection Count {ServerState.Connections.Count}");
                ServerState.ConnectionCount = ServerState.Connections.Count;
                if (ServerState.Connections.Count > 0)
                {
                    Debug.Log("Quitting: sending ExitServer and waiting.");
                    Util.GetServer().rpc.Invoke("ExitServer", new object[]
                    {
                Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
                    });
                    return false;
                }
                else
                {
                    return true;
                }
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
            Debug.Log($"Connection Count {ServerState.Connections.Count}");
            ServerState.ConnectionCount = ServerState.Connections.Count;
            if (ServerState.Connections.Count > 0)
            {
                Debug.Log("Logging out: sending ExitServer and waiting.");
                Util.GetServer().rpc.Invoke("ExitServer", new object[]
                {
                Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
                });
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool m_quitting;

        public static bool m_logging;
    }
}
