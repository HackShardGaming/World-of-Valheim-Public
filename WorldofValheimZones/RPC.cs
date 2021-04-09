
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BepInEx;
using HarmonyLib;


namespace WorldofValheimZones
{
    class RPC
    {

        public static void AddZone(long sender, ZPackage pkg)
        {
            string msg = pkg.ReadString(); // Read the message from the user.
            Debug.Log($"User is an admin and sent: {msg}");
            if (pkg != null && pkg.Size() > 0)
            { // Check that our Package is not null, and if it isn't check that it isn't empty.
                ZNetPeer peer = ZNet.instance.GetPeer(sender); // Get the Peer from the sender, to later check the SteamID against our Adminlist.
                if (peer != null)
                { // Confirm the peer exists
                    string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
                    if (
                        ZNet.instance.m_adminList != null &&
                        ZNet.instance.m_adminList.Contains(peerSteamID)
                    )
                    { // Check that the SteamID is in our Admin List.

                    }
                }
                else
                {
                    Debug.Log($"User is NOT an admin and sent: {msg}");
                }
            }
        }
    }
}
