
using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace WorldofValheimZones
{
    class CMethods
    {
        // Display our Help Menu
        public static void Help(Terminal __instance)
        {
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !getcoords (Show your current X and Y coords!)" }).GetValue();
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !addzone [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [r] (Add a zone to server **ADMIN COMMAND**)" }).GetValue();
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !addzone [Name] [ZoneType] [Priority] [Shape(circle/square)] [r] (Add a zone to the server using YOUR current location! **ADMIN COMMAND**)" }).GetValue();;
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !reload-zones (Reload all zones and update all users that are connected **ADMIN COMMAND**)" }).GetValue();
        }
        // Give the user their current coordinates (X Y).
        public static void GetCoords(Terminal __instance)
        {
            Vector3 point = Player.m_localPlayer.transform.position;
            Vector2 a = new Vector2(point.x, point.z);
            string x = a.x.ToString().Remove(a.x.ToString().Length - 3, 3);
            string y = a.y.ToString().Remove(a.y.ToString().Length - 3, 3);
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: Your current position is X: {x} Y: {y}" }).GetValue();
        }
        // Ask the server to reload all available zones **ADMIN ONLY**
        public static void ReloadZones()
        {
            ZPackage pkg = new ZPackage(); // Create ZPackage
            string msg = "reloadzones";
            pkg.Write(msg);
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "ReloadZones", new object[] { pkg });
            return;
        }
        // Ask the server to add a new zone to the server **ADMIN ONLY**
        public static void AddZone(Terminal __instance, string text)
        {
            string[] results = text.Split(' ');
            if (results.Count() == 6)
            {
                if (results[4].ToLower() == "circle" || results[4].ToLower() == "square")
                {
                    Vector3 point = Player.m_localPlayer.transform.position;
                    Vector2 a = new Vector2(point.x, point.z);
                    string Name = results[1];
                    string Type = results[2];
                    string Priority = results[3];
                    string Shape = results[4];
                    string X = a.x.ToString().Remove(a.x.ToString().Length - 3, 3);
                    string Y = a.y.ToString().Remove(a.y.ToString().Length - 3, 3);
                    string Radius = results[5];
                    string msg = $"{Name} {Type} {Priority} {Shape} {X} {Y} {Radius}";
                    ZPackage pkg = new ZPackage();
                    pkg.Write(msg);
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "AddZone", new object[] { pkg });
                    return;
                }
            }
            if (results.Count() == 8)
            {
                if (results[4].ToLower() == "circle" || results[4].ToLower() == "square")
                {
                    Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: Client->Server AddZone!" }).GetValue();
                    ZPackage pkg = new ZPackage(); // Create ZPackage
                    string msg = ""; // Make msg
                    for (int i = 1; i < results.Length; i++)
                    {
                        msg += results[i] + " ";
                    }
                    pkg.Write(msg); // Writes the msg to ZPackage
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "AddZone", new object[] { pkg });
                    return;
                }
            }
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: Error in formating. Please look below for proper formating" }).GetValue();
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !addzone [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [r]" }).GetValue();
            Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: !addzone [Name] [ZoneType] [Priority] [Shape(circle/square)] [r] (This will make a zone using your CURRENT location!)" }).GetValue();
            }
    }
}


#if client_cli

using BepInEx;
using static WorldofValheimZones.Console.CUtils;

namespace WorldofValheimZones.Console
{
    class CMethods
    {
        // Utils
        public static bool Help(string[] args)
        {
            Print("Commands: ");
            foreach (Command cmd in Runner.Instance.commands)
            {
                Print(cmd.Hint);
            }

            return true;
        }

        public static void SkipArg(string[] args)
        {
            string message = CombineArgs(args);
            if (!message.IsNullOrWhiteSpace())
            {
                Print($"Unnecessary argument skipped: {message}");
            }
        }

        // CLI Functions
        public static bool PrintVersion(string[] args)
        {
            SkipArg(args);
            Print($"Valheim Online version: {ModInfo.Version}");
            return true;
        }

        public static bool ZoneReload(string[] args)
        {
            SkipArg(args);
            Print($"Reloading Zones");
            ZoneHandler.LoadZoneData(WorldofValheimZones.ZonePath.Value);

            /*
            Util.GetServer().rpc.Invoke("ChatMessage", new object[] {
                new Vector3(), 1, "server", "Reloading Zones"
            });
            */

            Util.Broadcast("Reloading Zone");

            Debug.Log("S2C ZoneHandler (SendPeerInfo)");
            Util.GetServer().rpc.Invoke("ZoneHandler", new object[] {
                ZoneHandler.Serialize()
            });

            /*
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ZoneHandler",new object[] {
                ZoneHandler.Serialize()
            });

            /*
             *
             * rpc.Invoke("ZoneHandler", new object[] {
                ZoneHandler.Serialize()
            });
             */

            return true;
        }

    }
}

#endif