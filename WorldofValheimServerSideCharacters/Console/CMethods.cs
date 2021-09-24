using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldofValheimServerSideCharacters
{
    class CMethods
    {
        // Help Menu
        public static void Help(Terminal __instance)
        {
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !save (Saves your character (server side))" }).GetValue();
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !save-all (Request the server to save all clients **ADMIN COMMAND**)" }).GetValue();
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !server-shutdown (Shuts the server down **ADMIN COMMAND**)" }).GetValue();
        }
        // Save my character
        public static void Save(Terminal __instance)
        {
            if (Player.m_localPlayer != null)
            {
                Util.GetServer().rpc.Invoke("CharacterUpdate", new object[]
                {
                    Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
                });
                Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: Client->Server CharacterUpdate" }).GetValue();
            }
        }
        // Request that the server shuts itself down
        public static void ShutdownServer()
        {
            ZPackage pkg = new ZPackage(); // Create ZPackage
            string msg = "ShutdownServer";
            pkg.Write(msg);
            Debug.Log($"{ModInfo.Title}: C->S ShutdownServer");
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "ShutdownServer", new object[] { pkg });
        }
        // Save all online players.
        public static void SaveAll()
        {
            ZPackage pkg = new ZPackage(); // Create ZPackage
            string msg = "SaveAll";
            pkg.Write(msg);
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "SaveAll", new object[] { pkg });
        }
        public static void ReloadDefault()
        {
            ZPackage pkg = new ZPackage();
            pkg.Write("ReloadDefault");
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "ReloadDefault", new object[] { pkg });
        }
    }
}

/* Disabling Until fixed
using static WorldofValheimServerSideCharacters.Console.CUtils;


namespace WorldofValheimServerSideCharacters.Console
{
    class CMethods
    {
        // Utils
        public static bool Help(string[] args)
        {
            Debug.Log("Commands: ");
            foreach (Command cmd in Console.Instance.commands)
            {
                Debug.Log(cmd.Hint);
            }

            return true;
        }

        public static void SkipArg(string[] args)
        {
            string message = CombineArgs(args);
            if (!message.IsNullOrWhiteSpace())
            {
                Debug.Log($"Unnecessary argument skipped: {message}");
            }
        }

        // CLI Functions
        public static bool PrintVersion(string[] args)
        {
            SkipArg(args);
            Debug.Log($"Valheim Server Side Characters version: {ModInfo.Version}");
            return true;
        }
        public static bool Shutdown(string[] args)
        {
            SkipArg(args);
            Debug.Log($"Shutting down the server");
            Util.ShutdownServer();
            return true;
        }

        public static bool SaveAll(string[] args)
        {
            SkipArg(args);
            Debug.Log($"Saving all characters");
            Util.SaveAll();
            return true;
        }


    }
}
*/