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

namespace ValheimPermissions
{


    public static class Util
    {
        public static class Dedicated_Commands
        {
            public static void AddGroup(string parse)
            {
                bool results = ValheimDB.AddGroup(parse);
                if (results)
                    Debug.Log($"The group {parse} has been created");
                else
                    Debug.Log($"The group {parse} already exists!");

            }
            public static void AddUserPermission(long SteamID, string parse)
            {
                bool results = ValheimDB.AddUserPermission(SteamID.ToString(), parse.ToLower());
                if (!results)
                    Debug.Log($"ERROR: The user: {SteamID} already has access to the node: {parse}");
                else
                    Debug.Log($"The user: {SteamID} now has access to: {parse}");
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
        public static bool isServer()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }
        public static bool isAdmin(long steamid)
        {
            string SteamID = steamid.ToString();
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
        public static void ClientMessage(Console __instance, string message)
        {
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: {message}" }).GetValue();
        }
    }
}