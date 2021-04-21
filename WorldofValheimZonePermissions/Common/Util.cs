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
    class Util
    {
        private const int AreaRange = 100;
        private static int EffectTick = 0;
        private static Harmony harm = new Harmony("PrivateAreaKG");

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
            if (WorldofValheimZonePermissions.PrivateAreaKG.ContainsKey(ZoneType))
            {
                key = WorldofValheimZonePermissions.PrivateAreaKG[ZoneType].configs;
            }
            if (key.ToLower().Contains(restriction))
                return true;
            else
                return false;
        }

        public static void DoAreaEffect(Vector3 pos)
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
    }
}
