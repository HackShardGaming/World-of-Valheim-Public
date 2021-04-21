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
    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class WorldofValheimZonePermissions : BaseUnityPlugin
    {
        private const int AreaRange = 100;
        private static int HealTick = 0;
        private static int DamageTick = 0;
        private static int EffectTick = 0;
        private string hashCheck = "";
        public static Harmony harm = new Harmony("PrivateAreaKG");
        public static Dictionary<string, AreaInfo> PrivateAreaKG = new Dictionary<string, AreaInfo>();
        private static WorldofValheimZonePermissions plugin;
        public static ConfigEntry<string> ZonePermissionPath;
        public static ConfigEntry<int> NexusID;
        public class AreaInfo
        {
            public string configs;
        }
        static void OnDestroy()
        {
            harm.UnpatchSelf();
        }

        private static FileSystemWatcher watcher;
        void Awake()
        {
            plugin = this;
            harm.PatchAll();
            bool SERVER = Paths.ProcessName.Equals("valheim_server", StringComparison.OrdinalIgnoreCase) ? true : false;
            string testpath = BepInEx.Paths.ConfigPath;
            testpath = Path.Combine(testpath, "WoV");
            string path = Path.Combine(testpath, "ZonePermissions.txt");
            WorldofValheimZonePermissions.NexusID = base.Config.Bind<int>("WorldofValheimZonePermissions", "NexusID", ModInfo.NexusID, "Nexus ID to make Nexus Update Happy!");
            if (SERVER)
            {
                WorldofValheimZonePermissions.ZonePermissionPath = base.Config.Bind<string>("WorldofValheimZonePermissions", "ZonePermissionPath", path, "SERVER ONLY: Location of the ZonesPermissions file.");
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
                else
                {
                    hashCheck = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLower();
                }
                watcher = new FileSystemWatcher(Path.GetDirectoryName(WorldofValheimZonePermissions.ZonePermissionPath.Value));
                watcher.IncludeSubdirectories = true;
                ZLog.Log("STARTED WATCHER AT " + Path.GetDirectoryName(WorldofValheimZonePermissions.ZonePermissionPath.Value));
                watcher.Changed += OnChangedAREA;
                watcher.Filter = "ZonePermissions.txt";
                watcher.EnableRaisingEvents = true;
            }
        }
        private static void OnChangedAREA(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            ZLog.Log("AREA FILE CHANGED");
            if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
            {
                string path = WorldofValheimZonePermissions.ZonePermissionPath.Value;
                List<string> allText = File.ReadAllLines(path).ToList();
                foreach (var p in ZNet.instance.m_peers)
                {
                    ZPackage newPkg = new ZPackage();
                    string steam = ((ZSteamSocket)p.m_socket).GetPeerID().m_SteamID.ToString();
                    newPkg.Write(allText.Count);
                    for (int i = 0; i < allText.Count; i++)
                    {
                        if (allText[i] != "" && allText[i] != null && !allText[i].StartsWith("/") && allText[i] != string.Empty)
                        {
                            string[] array = allText[i].Replace(" ", "").Split('|');
                            string ZoneType = array[0];
                            string data = array[1];
                            string configs = array[2];
                            if (!data.Contains(steam))
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
                    ZRoutedRpc.instance.InvokeRoutedRPC(p.m_uid, "DownloadAreasServerKGMOD",
                        new object[] { newPkg });
                }
            }

        }
        public static bool RestrictionCheck(string restriction)
        {
            Player p = Player.m_localPlayer;
                // Are we in a zone? if so select that zone.
                WorldofValheimZones.ZoneHandler.Zone z = new WorldofValheimZones.ZoneHandler.Zone();
                WorldofValheimZones.ZoneHandler.ZoneTypes zt = new WorldofValheimZones.ZoneHandler.ZoneTypes();
                List<WorldofValheimZones.ZoneHandler.Zone> zlist = WorldofValheimZones.ZoneHandler.ListOccupiedZones(p.transform.position);
                string ZoneType = "wilderness";
                if (zlist.Count == 0)
                {
                    zt = WorldofValheimZones.ZoneHandler.FindZoneType("wilderness");
                    
                }
                else
                {
                    z = WorldofValheimZones.ZoneHandler.TopZone(zlist);
                    zt = WorldofValheimZones.ZoneHandler.FindZoneType(z.Type);
                    ZoneType = z.Type;
                }
                
                string key = "";
                if (PrivateAreaKG.ContainsKey(ZoneType))
                {
                    key = PrivateAreaKG[ZoneType].configs;
                }
            if (key.ToLower().Contains(restriction))
                return true;
            else
                return false;
        }
        //chat
        void FixedUpdate()
        { 
            if (EffectTick > 0) EffectTick--;
            if (HealTick > 0) HealTick--;
            if (DamageTick > 0) DamageTick--;
            Player p = Player.m_localPlayer;
            if (p != null)
            {
                // Are we in a zone? if so select that zone.
                WorldofValheimZones.ZoneHandler.Zone z = new WorldofValheimZones.ZoneHandler.Zone();
                WorldofValheimZones.ZoneHandler.ZoneTypes zt = new WorldofValheimZones.ZoneHandler.ZoneTypes();
                List<WorldofValheimZones.ZoneHandler.Zone> zlist = WorldofValheimZones.ZoneHandler.ListOccupiedZones(p.transform.position);
                string ZoneType = "wilderness";
                if (zlist.Count == 0)
                {
                    zt = WorldofValheimZones.ZoneHandler.FindZoneType("wilderness");
                    
                }
                else
                {
                    z = WorldofValheimZones.ZoneHandler.TopZone(zlist);
                    zt = WorldofValheimZones.ZoneHandler.FindZoneType(z.Type);
                    ZoneType = z.Type;
                }
                
                string key = "";
                if (PrivateAreaKG.ContainsKey(ZoneType))
                {
                    key = PrivateAreaKG[ZoneType].configs;
                }

                if (key.ToLower().Contains("pushaway"))
                {
                    Vector3 zposition = new Vector3(z.Position.x, p.transform.position.y, z.Position.y);
                    Vector3 newVector3 = p.transform.position +
                                         (p.transform.position - zposition).normalized * 0.15f;
                    p.transform.position = new Vector3(newVector3.x, p.transform.position.y, newVector3.z);
                }
                if (key.ToLower().Contains("periodicdamage") && DamageTick <= 0)
                {
                    DamageTick = 100;
                    string s = key.ToLower();
                    int indexStart = s.IndexOf("periodicdamage(") + "periodicdamage(".Length;
                    string test = "";
                    for (int i = indexStart; i < indexStart + 20; i++)
                    {
                        if (s[i] == ')') break;
                        test += s[i];
                    }
                    int damage = 0;
                    int.TryParse(test, out damage);
                    HitData hit = new HitData();
                    hit.m_damage.m_fire = (float)damage;
                    p.Damage(hit);
                }
                if (key.ToLower().Contains("periodicheal") && HealTick <= 0)
                {
                    HealTick = 50;
                    string s = key.ToLower();
                    int indexStart = s.IndexOf("periodicheal(") + "periodicheal(".Length;
                    string test = "";
                    for (int i = indexStart; i < indexStart + 20; i++)
                    {
                        if (s[i] == ')') break;
                        test += s[i];
                    }
                    int damage = 0;
                    int.TryParse(test, out damage);
                    p.Heal(damage, true);
                }
            }
        }

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
  
        public static void DownloadPAreasStart(long sender, ZPackage pkg)
        {
            if (pkg != null && pkg.Size() > 0 && !ZNet.instance.IsServer() && !ZNet.instance.IsDedicated())
            {
                PrivateAreaKG.Clear();
                int Count = pkg.ReadInt();
                for (int i = 0; i < Count; i++)
                {
                    if (pkg.ReadBool())
                    {
                        string ZoneType = pkg.ReadString();
                        string configs = pkg.ReadString();
                        AreaInfo info = new AreaInfo();
                        info.configs = configs;
                        PrivateAreaKG.Add(ZoneType, info);
                        //print($"ADDED AREA {area},{info.range},{info.configs}");
                    }
                }
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


        static void DoAreaEffect(Vector3 pos)
        {
            if (EffectTick <= 0)
            {
                EffectTick = 120;
                GameObject znet = ZNetScene.instance.GetPrefab("vfx_lootspawn");
                GameObject obj = UnityEngine.Object.Instantiate(znet, pos, Quaternion.identity);
                DamageText.WorldTextInstance worldTextInstance = new DamageText.WorldTextInstance();
                worldTextInstance.m_worldPos = pos;
                worldTextInstance.m_gui = UnityEngine.Object.Instantiate<GameObject>(DamageText.instance.m_worldTextBase, DamageText.instance.transform);
                worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<Text>();
                DamageText.instance.m_worldTexts.Add(worldTextInstance);
                worldTextInstance.m_textField.color = Color.cyan;
                worldTextInstance.m_textField.fontSize = 24;
                worldTextInstance.m_textField.text = "PRIVATE AREA";
                worldTextInstance.m_timer = -2f;
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
                if (RestrictionCheck("nopickaxe"))
                {
                    isInArea = true;
                    DoAreaEffect(hitPoint + Player.m_localPlayer.transform.forward * 1f);
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

                if (RestrictionCheck("nochest"))
                {
                    isInArea = true;
                    DoAreaEffect(__instance.transform.position);
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
                if (RestrictionCheck("nodoors"))
                {
                    isInArea = true;
                    DoAreaEffect(__instance.transform.position);
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
                if (RestrictionCheck("nobuilding"))
                {
                    isInArea = true;
                    DoAreaEffect(__instance.m_placementGhost.transform.position);
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
                if (RestrictionCheck("nobuilding"))
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
                if (RestrictionCheck("nobuilddamage"))
                {
                    isInArea = true;
                    DoAreaEffect(__instance.transform.position + Vector3.up * 0.5f);
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
                if (RestrictionCheck("noitemdrop"))
                {
                    isInArea = true;
                    DoAreaEffect(Player.m_localPlayer.transform.position + Vector3.up * 0.5f);
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
                if (RestrictionCheck("noitemdrop"))
                {
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        isInArea = true;
                        DoAreaEffect(Player.m_localPlayer.transform.position + Vector3.up * 0.5f);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                    }
                }
                return !isInArea;
            }
        }
    }
}
