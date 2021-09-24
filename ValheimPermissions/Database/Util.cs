using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using LiteDB;
using System.Linq;

namespace ValheimPermissions
{


    public static class Util
    {
        public static void ClientSideCommand(string text)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "ClientSideCommands", text);
        }
        public static void RoutedBroadcast(long peer, string text, string username = ModInfo.Title)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(peer, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }
        public static bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
        public static string GetPeerSteamID(string ID)
        {
            string Updated = ID.Replace("/", " ");
            List<ZNet.PlayerInfo> OnlinePlayers = ZNet.instance.GetPlayerList();
            bool PlayerExists = false;
            string CurrentPlayer = string.Empty;
            for (int i = 0; i < OnlinePlayers.Count; i++)
            {
                CurrentPlayer = OnlinePlayers[i].m_name;
                if (CurrentPlayer == Updated)
                {
                    PlayerExists = true;
                }
            }
            if (PlayerExists)
            {
                ZNetPeer peer = ZNet.instance.GetPeerByPlayerName(Updated);
                string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                return peerSteamID;
            }
            else
            {
                if ((ID.ToString().Length == 17) && IsAllDigits(ID.ToString()))
                {
                    ZNetPeer peer = ZNet.instance.GetPeer(long.Parse(ID));
                    string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                    return peerSteamID;
                }
            }
            return "00000000000000000";
        }
        public static bool IsAdmin(long sender)
        {
            string SteamID = sender.ToString();
            if (
                ZNet.instance.m_adminList != null &&
                ZNet.instance.m_adminList.Contains(SteamID)
            )
                return true;
            else
            {
                return false;
            }
        }
        public static class Dedicated_Commands
        {
            public static class ClientSideCommands
            {
                public static void ShowUserPermissions(long sender, long SteamID)
                {
                    string[] results = ValheimDB.ShowUserPermissions(SteamID.ToString());
                    int count = results.Length;
                    int i = 0;
                    Util.RoutedBroadcast(sender, $"The user {SteamID} has access to the following permission nodes:");
                    while (i < count)
                    {
                        Util.RoutedBroadcast(sender, results[i]);
                        i++;
                    }
                    string Group_Name = ValheimDB.GetGroup(SteamID.ToString());
                    if (Group_Name != null)
                    {
                        Util.RoutedBroadcast(sender, $"The user {SteamID} is also in the following group: {Group_Name}");
                        ShowGroupPermissions(sender, Group_Name);
                    }
                }
                public static void ShowGroupPermissions(long sender, string Group_Name)
                {
                    string[] results = ValheimDB.ShowGroupPermissions(Group_Name);
                    int count = results.Length;
                    int i = 0;
                    Util.RoutedBroadcast(sender, $"The group {Group_Name} has access to the following permission nodes:");
                    while (i < count)
                    {
                        Util.RoutedBroadcast(sender, results[i]);
                        i = i + 1;
                    }
                }
                public static void AddGroup(long sender, string parse)
                {
                    bool results = ValheimDB.AddGroup(parse);
                    if (results)
                        Util.RoutedBroadcast(sender, $"The group {parse} has been created");
                    else
                        Util.RoutedBroadcast(sender, $"The group {parse} already exists!");

                }
                public static void DelGroup(long sender, string parse)
                {
                    bool results = ValheimDB.DelGroup(parse);
                    if (results)
                        Util.RoutedBroadcast(sender, $"The group: {parse} and all permissions and users associated with this group have been deleted!");
                    else
                        Util.RoutedBroadcast(sender, $"The group: {parse} does not exist!");

                }
                /*
                public static void DeleteGroup(string parse)
                {
                    bool results = ValheimDB.DeleteGroup(parse);
                    if (results)
                        Debug.Log($"The deletion has been executed.");
                    else
                        Debug.Log($"The Deletion failed!");
                }
                */
                public static void AddUserPermission(long sender, long SteamID, string parse)
                {
                    bool results = ValheimDB.AddUserPermission(SteamID.ToString(), parse.ToLower());
                    if (!results)
                        Util.RoutedBroadcast(sender, $"ERROR: The user: {SteamID} already has access to the node: {parse}");
                    else
                        Util.RoutedBroadcast(sender, $"The user: {SteamID} now has access to: {parse}");
                }
                public static void DeleteUserPermission(long sender, long SteamID, string parse)
                {
                    bool results = ValheimDB.DelUserPermission(SteamID.ToString(), parse.ToLower());
                    if (!results)
                        Util.RoutedBroadcast(sender, $"ERROR: Deletion Failed! The user: {SteamID} does not have access to the node: {parse}");
                    else
                        Util.RoutedBroadcast(sender, $"The user: {SteamID} no longer has access to the node: {parse}");
                }
                public static void AddGroupPermission(long sender, string groupname, string parse)
                {
                    string results = ValheimDB.AddGroupPermission(groupname, parse.ToLower());
                    if (results == "false group")
                        Util.RoutedBroadcast(sender, $"ERROR: The group requested {groupname} does not exist!");
                    else if (results == "false exists")
                        Util.RoutedBroadcast(sender, $"ERROR: The group requested {groupname} already has access to the node {parse}");
                    else if (results == "true")
                        Util.RoutedBroadcast(sender, $"The group {groupname} now has access to {parse}");
                }
                public static void DelGroupPermission(long sender, string groupname, string parse)
                {
                    bool results = ValheimDB.DelGroupPermission(groupname, parse.ToLower());
                    if (!results)
                        Util.RoutedBroadcast(sender, $"ERROR: The group: {groupname} does not access to the node {parse}!");
                    else
                        Util.RoutedBroadcast(sender, $"The group: {groupname} no longer has access to the node {parse}");
                }
                public static void AddUserToGroup(long sender, long steamid, string parse)
                {
                    Debug.Log($"Attempting to add the user: {steamid} to the group: {parse}!");
                    string results = ValheimDB.AddUserToGroup(steamid.ToString(), parse);
                    if (results == "false group")
                        Util.RoutedBroadcast(sender, $"The requested group {parse} does not exist!");
                    else if (results == "false user")
                        Util.RoutedBroadcast(sender, $"The requested user {steamid} is already in a group!");
                    else if (results == "true")
                        Util.RoutedBroadcast(sender, $"The requested group {parse} has been added to the user {steamid}");
                }

                public static void CheckGroup(long sender, long steamid)
                {
                    string results = ValheimDB.GetGroup(steamid.ToString());
                    Util.RoutedBroadcast(sender, $"The user: {steamid} is in the Group: {results}");
                }
                public static void CheckUserPermission(long sender, long steamid, string permission)
                {
                    bool results = ValheimDB.CheckUserPermission(steamid.ToString(), permission);
                    if (results)
                        Util.RoutedBroadcast(sender, $"The user: {steamid.ToString()} has access to:{permission}");
                    else
                        Util.RoutedBroadcast(sender, $"The user: {steamid.ToString()} does not have access to: {permission}");
                }
                public static void CheckGroupPermission(long sender, string groupname, string permission)
                {
                    bool results = ValheimDB.PermissionScanGroup(groupname, permission);
                    if (results)
                        Util.RoutedBroadcast(sender, $"The group: {groupname} has access to: {permission}");
                    else
                        Util.RoutedBroadcast(sender, $"The group: {groupname} does not have access to: {permission}");
                }
            }
            public static class ServerSideConsole
            {

                public static void ShowUserPermissions(long SteamID)
                {
                    string[] results = ValheimDB.ShowUserPermissions(SteamID.ToString());
                    int count = results.Length;
                    int i = 0;
                    Debug.Log($"The user {SteamID} has access to the following permission nodes:");
                    while (i < count)
                    {
                        Debug.Log(results[i]);
                        i++;
                    }
                    string Group_Name = ValheimDB.GetGroup(SteamID.ToString());
                    if (Group_Name != null)
                    {
                        Debug.Log($"The user {SteamID} is also in the following group: {Group_Name}");
                        ShowGroupPermissions(Group_Name);
                    }
                }
                public static void ShowGroupPermissions(string Group_Name)
                {
                    string[] results = ValheimDB.ShowGroupPermissions(Group_Name);
                    int count = results.Length;
                    int i = 0;
                    Debug.Log($"The group {Group_Name} has access to the following permission nodes:");
                    while (i < count)
                    {
                        Debug.Log(results[i]);
                        i++;
                    }
                }
                public static void AddGroup(string parse)
                {
                    bool results = ValheimDB.AddGroup(parse);
                    if (results)
                        Debug.Log($"The group {parse} has been created");
                    else
                        Debug.Log($"The group {parse} already exists!");

                }
                public static void DelGroup(string parse)
                {
                    bool results = ValheimDB.DelGroup(parse);
                    if (results)
                        Debug.Log($"The group: {parse} and all permissions and users associated with this group have been deleted!");
                    else
                        Debug.Log($"The group: {parse} does not exist!");

                }
                /*
                public static void DeleteGroup(string parse)
                {
                    bool results = ValheimDB.DeleteGroup(parse);
                    if (results)
                        Debug.Log($"The deletion has been executed.");
                    else
                        Debug.Log($"The Deletion failed!");
                }
                */
                public static void AddUserPermission(long SteamID, string parse)
                {
                    bool results = ValheimDB.AddUserPermission(SteamID.ToString(), parse.ToLower());
                    if (!results)
                        Debug.Log($"ERROR: The user: {SteamID} already has access to the node: {parse}");
                    else
                        Debug.Log($"The user: {SteamID} now has access to: {parse}");
                }
                public static void DeleteUserPermission(long SteamID, string parse)
                {
                    bool results = ValheimDB.DelUserPermission(SteamID.ToString(), parse.ToLower());
                    if (!results)
                        Debug.Log($"ERROR: Deletion Failed! The user: {SteamID} does not have access to the node: {parse}");
                    else
                        Debug.Log($"The user: {SteamID} no longer has access to the node: {parse}");
                }
                public static void AddGroupPermission(string groupname, string parse)
                {
                    string results = ValheimDB.AddGroupPermission(groupname, parse.ToLower());
                    if (results == "false group")
                        Debug.Log($"ERROR: The group requested {groupname} does not exist!");
                    else if (results == "false exists")
                        Debug.Log($"ERROR: The group requested {groupname} already has access to the node {parse}");
                    else if (results == "true")
                        Debug.Log($"The group {groupname} now has access to {parse}");
                }
                public static void DelGroupPermission(string groupname, string parse)
                {
                    bool results = ValheimDB.DelGroupPermission(groupname, parse.ToLower());
                    if (!results)
                        Debug.Log($"ERROR: The group: {groupname} does not access to the node {parse}!");
                    else
                        Debug.Log($"The group: {groupname} no longer has access to the node {parse}");
                }
                public static void AddUserToGroup(long steamid, string parse)
                {
                    Debug.Log($"Attempting to add the user: {steamid} to the group: {parse}!");
                    string results = ValheimDB.AddUserToGroup(steamid.ToString(), parse);
                    if (results == "false group")
                        Debug.Log($"The requested group {parse} does not exist!");
                    else if (results == "false user")
                        Debug.Log($"The requested user {steamid} is already in a group!");
                    else if (results == "true")
                        Debug.Log($"The requested group {parse} has been added to the user {steamid}");
                }

                public static void CheckGroup(long steamid)
                {
                    string results = ValheimDB.GetGroup(steamid.ToString());
                    Debug.Log(results);
                }
                public static void CheckUserPermission(long steamid, string permission)
                {
                    bool results = ValheimDB.CheckUserPermission(steamid.ToString(), permission);
                    if (results)
                        Debug.Log($"The user: {steamid.ToString()} has access to:{permission}");
                    else
                        Debug.Log($"The user: {steamid.ToString()} does not have access to: {permission}");
                }
                public static void CheckGroupPermission(string groupname, string permission)
                {
                    bool results = ValheimDB.PermissionScanGroup(groupname, permission);
                    if (results)
                        Debug.Log($"The group: {groupname} has access to: {permission}");
                    else
                        Debug.Log($"The group: {groupname} does not have access to: {permission}");
                }
            }
        }
        public static bool IsServer()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }
        public static void ClientMessage(Terminal __instance, string message)
        {
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: {message}" }).GetValue();
        }
    }
}