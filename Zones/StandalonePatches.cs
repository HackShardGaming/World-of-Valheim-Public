using System;
using HarmonyLib;

#if client_cli
using WorldofValheimZones.Console;
#endif

namespace WorldofValheimZones
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
        
        //
        // This is the bread and butter of maintaining the user PVP state
        //
        // This class will constantly check for any updates in position and set the client PVP state accordingly.
        //
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game), "Update")]
        private static void Game__Update()
        {
            if (Player.m_localPlayer)
            {
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
                peer.m_rpc.Register<ZPackage>("ZoneHandler", new Action<ZRpc, ZPackage>(ZoneHandler.RPC));
                peer.m_rpc.Register<ZPackage>("Client", new Action<ZRpc, ZPackage>(Client.RPC));

                // Reset zone ID
                ZoneHandler.CurrentZoneID = -2;
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

            // Syncing Zone Handler Settings.
#if DEBUG
            Debug.Log("S2C ZoneHandler (SendPeerInfo)");
            ZoneHandler._debug();
#endif
            rpc.Invoke("ZoneHandler", new object[] {
                ZoneHandler.Serialize()
            });


            // Syncing the Client State with the server defaults.
#if DEBUG
            Debug.Log("S2C ClientState (SendPeerInfo)");
            Client._debug();
#endif
            rpc.Invoke("Client", new object[] {
                Client.Serialize()
            });
            

            Util.Connections.Add(new Util.ConnectionData
            {
                rpc = rpc
            });
        }
    }
}
