using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ValheimPermissions
{
    [HarmonyPatch]
    class StandAlonePatches
    {
        [HarmonyPatch(typeof(Game), "Start")]
        public static class GameStartPatch
        {
            private static void Prefix()
            {
                    Debug.Log("ClientSideCommands RPC Created");
                    ZRoutedRpc.instance.Register("ClientSideCommands", new Action<long, string>(RPC.ProcessClientSideCommand)); // Adding Zone
            }
        }
        
        [HarmonyPatch(typeof(Chat), nameof(Chat.OnNewChatMessage))]
        public static class ChatManager
        {
            private static void Prefix(ref Chat __instance, ref GameObject go, ref long senderID, ref Vector3 pos, ref Talker.Type type, ref string user, ref string text)
            {
                if (ValheimPermissions.ServerMode)
                {
                    /*
                    ZNetPeer peer = ZNet.instance.GetPeer(senderID);
                    string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
                    string permission = "hackshardgaming.valheimpermission.consoleaccess";
                    bool result = ValheimDB.CheckUserPermission(peerSteamID, permission);
                    if (result)
                    {
                        RPC.ProcessClientSideCommand(senderID, text);
                        return;
                    }
                    */
                }
                else
                {
                    if (senderID == ZRoutedRpc.instance.GetServerPeerID())
                    {
                        Debug.Loguntagged($"{user}: {text}");
                    }
                }
            }
        }
        //Note this is client side only console commands.
        [HarmonyPatch(typeof(Terminal), "InputText")]
        static class InputText_Patch
        {
            static bool Prefix(Terminal __instance)
            {
                string text = __instance.m_input.text;
                // Lets check the version!
                if (text.ToLower().Equals($"!version"))
                {
                    if (Player.m_localPlayer != null)
                    {
                        Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                        Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: {ModInfo.Version}" }).GetValue();
                        return false;
                    }
                    else
                        return true;
                }
                // Help Menu
                if (text.ToLower().StartsWith($"!valheimpermissions"))
                {
                    string command = text.Remove(0, 20);
                    Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: Sending command {command} to the server check your chat message for a reply" }).GetValue();
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "ClientSideCommands", command);
                    return false;
                }
                return true;
            }
        }
    }
}

