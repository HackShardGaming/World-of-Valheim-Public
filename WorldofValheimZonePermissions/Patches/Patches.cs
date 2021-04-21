using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Configuration;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;
using WorldofValheimZones;

namespace WorldofValheimZonePermissions
{
    class Patches
    {

        [HarmonyPatch(typeof(Chat), "InputText")]
        public static class Chat_Patch
        {
            private static bool Prefix(Chat __instance)
            {
                string text = __instance.m_input.text;
                string[] array = text.Split(' ');
                Player p = Player.m_localPlayer;
                Vector3 pos = p.transform.position;
                /*if (text.StartsWith("/privatearea") && !IsNearOtherAreas(pos))
                {
                    float rad = AreaRange;
                    ZPackage pkg = new ZPackage();
                    pkg.Write(pos);
                    pkg.Write(rad);

                    string ALLOWEDNAMES = "";
                    for (int i = 1; i < array.Length; i++)
                    {
                        if (array[i].TrimEnd(' ') != "") ALLOWEDNAMES += array[i].Trim(' ') + ",";
                    }
                    pkg.Write(ALLOWEDNAMES);
                    string configs = "NoBuilding NoBuildDamage NoChest NoPickaxe NoDoors";
                    pkg.Write(configs);
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "SendAreaToServerKGMOD", new object[] { pkg });
                    return false;
                }*/

                if (text == "/pos")
                {
                    Chat.instance.AddString($"Position = {pos}");
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ZoneSystem), "Start")]
        public static class GameStartPatch
        {
            private static void Prefix()
            {
                ZRoutedRpc.instance.Register("DownloadAreasServerKGMOD", new Action<long, ZPackage>(WorldofValheimZonePermissions.DownloadPAreasStart));
            }
        }

        [HarmonyPatch(typeof(ZNet), "RPC_CharacterID")]
        public static class RPC_PATCH
        {
            private static void Postfix(ZNet __instance, ZRpc rpc)
            {
                if (!__instance.IsDedicated() && !__instance.IsServer())
                {
                    return;
                }

                ZNetPeer peer = __instance.GetPeer(rpc);
                string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                string path = WorldofValheimZonePermissions.ZonePermissionPath.Value;
                List<string> allText = File.ReadAllLines(path).ToList();
                ZPackage newPkg = new ZPackage();
                newPkg.Write(allText.Count);
                for (int i = 0; i < allText.Count; i++)
                {
                    if (allText[i] != "" && allText[i] != null && !allText[i].StartsWith("/") && allText[i] != string.Empty)
                    {
                        string[] array = allText[i].Replace(" ", "").Split('|');
                        string ZoneType = array[0];
                        string data = array[1];
                        string configs = array[2];
                        if (!data.Contains(peerSteamID))
                        {
                            newPkg.Write(true);
                            newPkg.Write(ZoneType);
                            newPkg.Write(configs);
                        }
                        else
                        {
                            newPkg.Write(false);
                        }
                    }
                    else
                    {
                        newPkg.Write(false);
                    }
                }
                ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "DownloadAreasServerKGMOD",
                    new object[] { newPkg });
            }
        }
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

        /////////////////PArea patches
        [HarmonyPatch(typeof(Attack), "SpawnOnHitTerrain")]
        public static class Attack_Patch
        {
            private static bool Prefix(Vector3 hitPoint)
            {
                bool isInArea = false;
                if (Util.RestrictionCheck("nopickaxe"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(hitPoint + Player.m_localPlayer.transform.forward * 1f);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        [HarmonyPatch(typeof(Container), "Interact")]
        public static class Container_Patch
        {
            private static bool Prefix(Container __instance)
            {
                bool isInArea = false;

                if (Util.RestrictionCheck("nochest"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(__instance.transform.position);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        [HarmonyPatch(typeof(Door), "Interact")]
        public static class Door_Patch
        {
            private static bool Prefix(Door __instance)
            {
                bool isInArea = false;
                if (Util.RestrictionCheck("nodoors"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(__instance.transform.position);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        [HarmonyPatch(typeof(Player), "PlacePiece")]
        public static class NoBuild_Patch
        {
            private static bool Prefix(Player __instance)
            {
                bool isInArea = false;
                if (Util.RestrictionCheck("nobuilding"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(__instance.m_placementGhost.transform.position);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        [HarmonyPatch(typeof(Player), "RemovePiece")]
        public static class NoBuild_Patch2
        {
            private static bool Prefix(Player __instance)
            {
                bool isInArea = false;
                if (Util.RestrictionCheck("nobuilding"))
                {
                    isInArea = true;
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);

                }
                return !isInArea;
            }
        }

        [HarmonyPatch(typeof(WearNTear), "RPC_Damage")]
        public static class NoBuild_Damage_Patch
        {
            private static bool Prefix(WearNTear __instance)
            {
                bool isInArea = false;
                if (Util.RestrictionCheck("nobuilddamage"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(__instance.transform.position + Vector3.up * 0.5f);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        [HarmonyPatch(typeof(InventoryGui), "OnDropOutside")]
        public static class NoDrop_Patch
        {
            private static bool Prefix(InventoryGui __instance)
            {
                bool isInArea = false;
                if (Util.RestrictionCheck("noitemdrop"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(Player.m_localPlayer.transform.position + Vector3.up * 0.5f);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        [HarmonyPatch(typeof(InventoryGrid), "OnLeftClick")]
        public static class NoDrop_Patch2
        {
            private static bool Prefix(InventoryGrid __instance)
            {
                bool isInArea = false;
                if (Util.RestrictionCheck("noitemdrop"))
                {
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        isInArea = true;
                        Util.DoAreaEffect(Player.m_localPlayer.transform.position + Vector3.up * 0.5f);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                    }
                }
                return !isInArea;
            }
        }
    }
}
