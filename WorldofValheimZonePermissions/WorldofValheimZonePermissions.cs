using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Configuration;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;

namespace WorldofValheimZonePermissions
{
    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class PArea : BaseUnityPlugin
    {
        private const int AreaRange = 100;
        private static int HealTick = 0;
        private static int DamageTick = 0;
        private static int EffectTick = 0;
        private string hashCheck = "";
        private static Harmony harm = new Harmony("PrivateAreaKG");
        private static Dictionary<Vector3, AreaInfo> PrivateAreaKG = new Dictionary<Vector3, AreaInfo>();
        private static PArea plugin;
        public class AreaInfo
        {
            public float range;
            public string configs;
        }


        //IEnumerator DetectHashChange(string path)
        //{
        //    for (;;)
        //    {
        //        yield return new WaitForSecondsRealtime(5f);
        //        string newHash = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLower();
        //        if (newHash != hashCheck)
        //        {
        //            hashCheck = newHash;

        //            List<string> allText = File.ReadAllLines(path).ToList();
        //            foreach (var p in ZNet.instance.m_peers)
        //            {
        //                ZPackage newPkg = new ZPackage();
        //                string steam = ((ZSteamSocket)p.m_socket).GetPeerID().m_SteamID.ToString();
        //                newPkg.Write(allText.Count);
        //                for (int i = 0; i < allText.Count; i++)
        //                {
        //                    if (allText[i] != "" && allText[i] != null && !allText[i].StartsWith("/") && allText[i] != string.Empty)
        //                    {
        //                        string[] array = allText[i].Replace(" ", "").Split('|');
        //                        Vector3 Vec = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
        //                        float range = float.Parse(array[3]);
        //                        string data = array[4];
        //                        string configs = array[5];
        //                        if (!data.Contains(steam))
        //                        {
        //                            newPkg.Write(true);
        //                            newPkg.Write(Vec);
        //                            newPkg.Write(range);
        //                            newPkg.Write(configs);
        //                        }
        //                        else
        //                        {
        //                            newPkg.Write(false);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        newPkg.Write(false);
        //                    }
        //                }
        //                ZRoutedRpc.instance.InvokeRoutedRPC(p.m_uid, "DownloadAreasServerKGMOD",
        //                    new object[] { newPkg });
        //            }

        //        }
        //    }
        //}
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
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PrivateAreas.cfg");
            if (SERVER)
            {
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
                else
                {
                    hashCheck = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLower();
                }
                watcher = new FileSystemWatcher(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                ZLog.Log("STARTED WATCHER AT " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                watcher.Changed += OnChangedAREA;
                watcher.Filter = "PrivateAreas.cfg";
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
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "PrivateAreas.cfg");
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
                            Vector3 Vec = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
                            float range = float.Parse(array[3]);
                            string data = array[4];
                            string configs = array[5];
                            if (!data.Contains(steam))
                            {
                                newPkg.Write(true);
                                newPkg.Write(Vec);
                                newPkg.Write(range);
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

        static bool IsNearOtherAreas(Vector3 v)
        {
            foreach (var area in PrivateAreaKG)
            {
                if (Vector3.Distance(area.Key, v) <= area.Value.range * 2)
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "<color=red>Other private area near!</color>", 0, null);
                    return true;
                }
            }

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
                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, p.transform.position) <= key.Value.range)
                    {
                        if (key.Value.configs.ToLower().Contains("pushaway"))
                        {
                            Vector3 newVector3 = p.transform.position +
                                                 (p.transform.position - key.Key).normalized * 0.15f;
                            p.transform.position = new Vector3(newVector3.x, p.transform.position.y, newVector3.z);
                        }
                        if (key.Value.configs.ToLower().Contains("pveonly"))
                        {
                            p.SetPVP(false);
                            p.m_lastCombatTimer = 0;
                            InventoryGui.instance.m_pvp.isOn = false;
                            break;
                        }
                        if (key.Value.configs.ToLower().Contains("pvponly"))
                        {
                            p.SetPVP(true);
                            p.m_lastCombatTimer = 0;
                            InventoryGui.instance.m_pvp.isOn = true;
                            break;
                        }

                        if (key.Value.configs.ToLower().Contains("periodicdamage") && DamageTick <= 0)
                        {
                            DamageTick = 100;
                            string s = key.Value.configs.ToLower();
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
                        if (key.Value.configs.ToLower().Contains("periodicheal") && HealTick <= 0)
                        {
                            HealTick = 50;
                            string s = key.Value.configs.ToLower();
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

        static void SendAreaToServer(long sender, ZPackage pkg)
        {
            if (pkg != null && pkg.Size() > 0 && ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PrivateAreas.cfg");
                List<string> allText = File.ReadAllLines(path).ToList();
                ZNetPeer peer = ZRoutedRpc.instance.GetPeer(sender);
                string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                Vector3 vec = pkg.ReadVector3();
                float rad = pkg.ReadSingle();
                string ALLOWEDMAIN = pkg.ReadString();
                string configs = pkg.ReadString();
                string ALLOWED = peerSteamID;

                foreach (var s in ALLOWEDMAIN.Split(',').ToArray())
                {
                    ZNetPeer pee = ZNet.instance.GetPeerByPlayerName(s);
                    if (pee != null)
                    {
                        string STEAM = ((ZSteamSocket)pee.m_socket).GetPeerID().m_SteamID.ToString();
                        ALLOWED += " " + STEAM;
                    }
                }
                string newS = $"{vec.x} | {vec.y} | {vec.z} | {rad} | {ALLOWED} | {configs}";
                bool add = true;
                foreach (var s in allText.ToArray())
                {
                    if (!s.StartsWith("/") && !s.StartsWith(" ") && s != string.Empty)
                    {
                        string[] newsplit = s.Replace(" ", "").Split('|');
                        if (newsplit[4] == peerSteamID)
                        {
                            add = false;
                            int index = allText.IndexOf(s);
                            allText[index] = newS;
                            break;
                        }
                    }
                }
                if (!add)
                {
                    File.WriteAllLines(path, allText);
                }
                else
                {
                    allText.Add(newS);
                    File.WriteAllLines(path, allText);
                }
            }
        }


        static void DownloadPAreasStart(long sender, ZPackage pkg)
        {
            if (pkg != null && pkg.Size() > 0 && !ZNet.instance.IsServer() && !ZNet.instance.IsDedicated())
            {
                PrivateAreaKG.Clear();
                int Count = pkg.ReadInt();
                for (int i = 0; i < Count; i++)
                {
                    if (pkg.ReadBool())
                    {
                        Vector3 area = pkg.ReadVector3();
                        float range = pkg.ReadSingle();
                        string configs = pkg.ReadString();
                        AreaInfo info = new AreaInfo();
                        info.range = range;
                        info.configs = configs;
                        PrivateAreaKG.Add(area, info);
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
                ZRoutedRpc.instance.Register("DownloadAreasServerKGMOD", new Action<long, ZPackage>(PArea.DownloadPAreasStart));
                ZRoutedRpc.instance.Register("SendAreaToServerKGMOD", new Action<long, ZPackage>(PArea.SendAreaToServer));
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
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PrivateAreas.cfg");
                List<string> allText = File.ReadAllLines(path).ToList();
                ZPackage newPkg = new ZPackage();
                newPkg.Write(allText.Count);
                for (int i = 0; i < allText.Count; i++)
                {
                    if (allText[i] != "" && allText[i] != null && !allText[i].StartsWith("/") && allText[i] != string.Empty)
                    {
                        string[] array = allText[i].Replace(" ", "").Split('|');
                        Vector3 Vec = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
                        float range = float.Parse(array[3]);
                        string data = array[4];
                        string configs = array[5];
                        if (!data.Contains(peerSteamID))
                        {
                            newPkg.Write(true);
                            newPkg.Write(Vec);
                            newPkg.Write(range);
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


        /////////////////PArea patches
        [HarmonyPatch(typeof(Attack), "SpawnOnHitTerrain")]
        public static class Attack_Patch
        {
            private static bool Prefix(Vector3 hitPoint)
            {
                bool isInArea = false;
                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, hitPoint) <= key.Value.range && key.Value.configs.ToLower().Contains("nopickaxe"))
                    {
                        isInArea = true;
                        DoAreaEffect(hitPoint + Player.m_localPlayer.transform.forward * 1f);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                        break;
                    }
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

                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, __instance.transform.position) <= key.Value.range && key.Value.configs.ToLower().Contains("nochest"))
                    {
                        isInArea = true;
                        DoAreaEffect(__instance.transform.position);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                        break;
                    }
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

                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, __instance.transform.position) <= key.Value.range && key.Value.configs.ToLower().Contains("nodoors"))
                    {
                        isInArea = true;
                        DoAreaEffect(__instance.transform.position);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                        break;
                    }
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

                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, __instance.m_placementGhost.transform.position) <= key.Value.range && key.Value.configs.ToLower().Contains("nobuilding"))
                    {
                        isInArea = true;
                        DoAreaEffect(__instance.m_placementGhost.transform.position);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                        break;
                    }
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

                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, __instance.transform.position) <= key.Value.range && key.Value.configs.ToLower().Contains("nobuilding"))
                    {
                        isInArea = true;
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                        break;
                    }
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

                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, __instance.transform.position) <= key.Value.range && key.Value.configs.ToLower().Contains("nobuilddamage"))
                    {
                        isInArea = true;
                        DoAreaEffect(__instance.transform.position + Vector3.up * 0.5f);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                        break;
                    }
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

                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, Player.m_localPlayer.transform.position) <= key.Value.range && key.Value.configs.ToLower().Contains("noitemdrop"))
                    {
                        isInArea = true;
                        DoAreaEffect(Player.m_localPlayer.transform.position + Vector3.up * 0.5f);
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                        break;
                    }
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

                foreach (KeyValuePair<Vector3, AreaInfo> key in PrivateAreaKG)
                {
                    if (Vector3.Distance(key.Key, Player.m_localPlayer.transform.position) <= key.Value.range && key.Value.configs.ToLower().Contains("noitemdrop"))
                    {
                        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                        {
                            isInArea = true;
                            DoAreaEffect(Player.m_localPlayer.transform.position + Vector3.up * 0.5f);
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                            break;
                        }
                        
                    }
                }
                return !isInArea;
            }
        }
    }
}
