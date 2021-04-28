using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ValheimPermissions
{
    class ChatListener
    {
        [HarmonyPatch(typeof(Chat), "OnNewChatMessage")]
        internal class OnNewChatMessage
        {
            private static bool Prefix(ref long senderID, ref string user, ref string text, ref Talker.Type type)
            {
                if (text.ToLower().StartsWith("!user") || text.ToLower().StartsWith("!group"))
                {
                    RPC.ProcessClientSideCommand(senderID, text);
                }

                return true;
            }

        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Chat), "OnNewChatMessage")]
        private static void Chat__InputText(ref Chat __instance)
        {
            Debug.Log($"Found some input {__instance.m_input.text}");
            ZNetPeer peer = __instance.GetComponent<ZNetPeer>();
            long sender = long.Parse(((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString());
            var text = __instance.m_input.text;
            if (text.ToLower().StartsWith("!user") || text.ToLower().StartsWith("!group"))
            {
                
            }
        }
    }
}
