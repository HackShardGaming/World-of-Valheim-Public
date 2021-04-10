using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;

namespace WorldofValheimZones
{
    //Note this is client side only console commands.
    [HarmonyPatch(typeof(Console), "InputText")]
    static class F5Console
    {
        static bool Prefix(Console __instance)
        {
            string text = __instance.m_input.text;

            // AM I a Player?
            if (Player.m_localPlayer != null)
            {
                // Version results.
                if (text.ToLower().Equals($"!version"))
                {

                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones Version: {ModInfo.Version}" }).GetValue();
                    return false;
                }
                // Add Zone
                if (text.ToLower().StartsWith($"!getcoords"))
                {
                    Vector3 point = Player.m_localPlayer.transform.position;
                    Vector2 a = new Vector2(point.x, point.z);
                    Traverse.Create(__instance).Method("AddString", new object[] { $"Your current position is X: {a.x} Y: {a.y}" }).GetValue();
                }
                
                if (text.ToLower().StartsWith($"!reload-zones"))
                {
                    ZPackage pkg = new ZPackage(); // Create ZPackage
                    string msg = "reloadzones";
                    pkg.Write(msg);
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "ReloadZones", new object[] { pkg });
                    return false;
                }
                if (text.ToLower().StartsWith($"!help"))
                {
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"!getcoords (Show your current X and Y coords!)" }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"!addzone [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [r] (Add a zone to server **ADMIN COMMAND**)" }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"!reload-zones (Reload all zones and update all users that are connected **ADMIN COMMAND**)" }).GetValue();
                }
                if (text.ToLower().StartsWith($"!addzone"))
                {
                    string[] results = text.Split(' ');
                    if (results.Count() == 8) {
                        if (results[4].ToLower() == "circle" || results[4].ToLower() == "square")
                        {
                            Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                            Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Client->Server AddZone!" }).GetValue();
                            ZPackage pkg = new ZPackage(); // Create ZPackage
                            string msg = ""; // Make msg
                            for (int i = 1; i < results.Length; i++)
                            {
                                msg += results[i] + " ";
                            }
                            pkg.Write(msg); // Writes the msg to ZPackage
                            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "AddZone", new object[] { pkg });

                            return false;
                        }
                    }
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Error in formating. The proper formating is !addzone [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [r]" }).GetValue();
                    return false;
                }
                // Del Zone
                if (text.ToLower().StartsWith($"!delzone"))
                {
                    /*
                    Util.GetServer().rpc.Invoke("delzone", new object[]
                    {
                    });
                    */
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Error: Proper Formating is !delzone [Name]" }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Clinet->Server DelZone" }).GetValue();
                    return false;

                }
                return true;
            }
            return true;
        }
    }
}
