using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using System.IO;


namespace WorldofValheimZones
{
    class Patches
    {
        //chat
        public static void FixedUpdatez()
        {
            if (WorldofValheimZones.EffectTick > 0) WorldofValheimZones.EffectTick--;
            if (WorldofValheimZones.HealTick > 0) WorldofValheimZones.HealTick--;
            if (WorldofValheimZones.DamageTick > 0) WorldofValheimZones.DamageTick--;
            Player p = Player.m_localPlayer;
            if (p != null)
            {
                 // Are we in a zone? if so select that zone.
                ZoneHandler.Zone z = new ZoneHandler.Zone();
                ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
                List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
                if (zlist.Count == 0)
                {
                    zt = ZoneHandler.FindZoneType("wilderness");

                }
                else
                {
                    z = ZoneHandler.TopZone(zlist);
                    zt = ZoneHandler.FindZoneType(z.Type);

                }
                // Lets set our strings...
                string admins = zt.Admins;
                string key = zt.Configurations;
                // Lets see if the user is considered a Zone Admin!
                if (admins.Contains(WorldofValheimZones.MySteamID))
                {
                    return;
                }
                // Keep player out of an area.
                if (key.ToLower().Contains("pushaway"))
                {
                    Vector3 zposition = new Vector3(z.Position.x, p.transform.position.y, z.Position.y);
                    Vector3 newVector3 = p.transform.position +
                                         (p.transform.position - zposition).normalized * 0.15f;
                    p.transform.position = new Vector3(newVector3.x, p.transform.position.y, newVector3.z);
                }
                // Deal damage to the player while they are in the area.
                if (key.ToLower().Contains("periodicdamage") && WorldofValheimZones.DamageTick <= 0)
                {
                    WorldofValheimZones.DamageTick = 100;
                    string s = key.ToLower();
                    int indexStart = s.IndexOf("periodicdamage(") + "periodicdamage(".Length;
                    string test = "";
                    for (int i = indexStart; i < indexStart + 30; i++)
                    {
                        if (s[i] == ')') break;
                        test += s[i];
                    }
                    int damage = 0;
                    HitData hit = new HitData();
                    string[] array = test.Split(',');
                    if (array.Count() == 2)
                    {
                        int.TryParse(array[1], out damage);
                        if (array[0].ToLower() == "fire")
                        {
                            hit.m_damage.m_fire = (float)damage;
                        }
                        else if (array[0].ToLower() == "frost")
                        {
                            hit.m_damage.m_frost = (float)damage;
                        }
                        else if (array[0].ToLower() == "poison")
                        {
                            hit.m_damage.m_poison = (float)damage;
                        }
                        else if (array[0].ToLower() == "lightning")
                        {
                            hit.m_damage.m_lightning = (float)damage;
                        }
                        else if (array[0].ToLower() == "pierce")
                        {
                            hit.m_damage.m_pierce = (float)damage;
                        }
                        else if (array[0].ToLower() == "blunt")
                        {
                            hit.m_damage.m_blunt = (float)damage;
                        }
                        else if (array[0].ToLower() == "slash")
                        {
                            hit.m_damage.m_slash = (float)damage;
                        }
                        else if (array[0].ToLower() == "damage")
                        {
                            hit.m_damage.m_damage = (float)damage;
                        }
                        else
                        {
                            hit.m_damage.m_fire = (float)damage;
                        }
                    }
                    else
                    {
                        int.TryParse(test, out damage);
                        hit.m_damage.m_fire = (float)damage;
                    }
                    p.Damage(hit);
                }
                // Heal the player while they are in the area
                if (key.ToLower().Contains("periodicheal") && WorldofValheimZones.HealTick <= 0)
                {
                    WorldofValheimZones.HealTick = 50;
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

        /// <summary>
        /// This is a list of our "Zone Configurations"
        /// </summary>
        /// No Pickaxe! (NoPickaxe)
        [HarmonyPatch(typeof(Attack), "SpawnOnHitTerrain")]
        public static class Attack_Patch
        {
            private static bool Prefix(Vector3 hitPoint)
            {
                bool isInArea = false;
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
                if (Util.RestrictionCheck("nopickaxe"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(hitPoint + Player.m_localPlayer.transform.forward * 1f);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        // No Chest Access! (NoChest)
        [HarmonyPatch(typeof(Container), "Interact")]
        public static class Container_Patch
        {
            private static bool Prefix(Container __instance)
            {
                bool isInArea = false;
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
                if (Util.RestrictionCheck("nochest"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(__instance.transform.position);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        // No Door Access! (NoDoors)
        [HarmonyPatch(typeof(Door), "Interact")]
        public static class Door_Patch
        {
            private static bool Prefix(Door __instance)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
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

        // No Placing Pieces Access! (NoBuilding)
        [HarmonyPatch(typeof(Player), "PlacePiece")]
        public static class NoBuild_Patch
        {
            private static bool Prefix(Player __instance)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
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

        // No Removing Pieces Access! (NoBuilding)
        [HarmonyPatch(typeof(Player), "RemovePiece")]
        public static class NoBuild_Patch2
        {
            private static bool Prefix(Player __instance)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
                bool isInArea = false;
                if (Util.RestrictionCheck("nobuilding"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(__instance.transform.position);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);

                }
                return !isInArea;
            }
        }

        // No Damaging Buildings (NoBuildDamage)
        [HarmonyPatch(typeof(WearNTear), "RPC_Damage")]
        public static class NoBuild_Damage_Patch
        {
            private static bool Prefix(WearNTear __instance)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
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

        // No Picking Up Items! (NoItemPickup)
        [HarmonyPatch(typeof(ItemDrop), "Interact")]
        public static class NoDropInteraction_Patch
        {
            private static bool Prefix(ItemDrop __instance)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
                bool isInArea = false;
                if (Util.RestrictionCheck("noitempickup"))
                {
                    isInArea = true;
                    Util.DoAreaEffect(__instance.transform.position + Vector3.up * 0.5f);
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "This is Private Area", 0, null);
                }
                return !isInArea;
            }
        }

        // No Picking up Items! (NoItemPickup)
        [HarmonyPatch(typeof(Player), "AutoPickup")]
        public static class NoAutoPickup_Patch
        {
            private static bool Prefix(Player __instance)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
                bool isInArea = false;
                if (Util.RestrictionCheck("noitempickup"))
                {
                    isInArea = true;
                }
                return !isInArea;
            }
        }
        // Damage Multipliers! (DamageMultiplierToMobs(1), DamageMultiplierToPlayers(1))
        [HarmonyPatch(typeof(Character), "Damage")]
        public static class Damage_Modifier
        {
            
            public static void Prefix(Character __instance, HitData hit)
            {
                long LocalPlayer = long.Parse("0000000");
                if (Player.m_localPlayer)
                    LocalPlayer = Player.m_localPlayer.GetZDOID().m_userID;
                long RemotePlayer = __instance.GetZDOID().m_userID;
#if DEBUG
                Debug.Log($"Local UID: {LocalPlayer} Remote UID: {RemotePlayer}");
#endif
                if (LocalPlayer == RemotePlayer)
                {
#if DEBUG
                    Debug.Log($"Checking SteamID: {(ZNet.instance.GetPeer(__instance.GetZDOID().m_userID)).m_socket.GetHostName()} against DamageMultiplierToPlayers");
#endif
                    if (Util.RestrictionCheck("damagemultipliertoplayers"))
                    {
#if DEBUG
                        Debug.Log($"User SteamID: {(ZNet.instance.GetPeer(__instance.GetZDOID().m_userID)).m_socket.GetHostName()} is restricted! Lets modify data");
#endif
                        float multiplier = Util.RestrictionCheckFloatReturn("damagemultipliertoplayers");
                        Debug.Log($"The multiplier is: {multiplier}");
                        hit.m_damage.m_damage *= multiplier;
                        hit.m_damage.m_blunt *= multiplier;
                        hit.m_damage.m_slash *= multiplier;
                        hit.m_damage.m_pierce *= multiplier;
                        hit.m_damage.m_chop *= multiplier;
                        hit.m_damage.m_pickaxe *= multiplier;
                        hit.m_damage.m_fire *= multiplier;
                        hit.m_damage.m_frost *= multiplier;
                        hit.m_damage.m_lightning *= multiplier;
                        hit.m_damage.m_poison *= multiplier;
                        hit.m_damage.m_spirit *= multiplier;
                        return;
                    }
                }
                else
                {
#if DEBUG
                    Debug.Log($"Instance is not a local player.  Lets see what it really is...");
#endif
                    if (__instance.m_faction != Character.Faction.Players)
                    {
#if DEBUG
                        Debug.Log($"A monster is being attacked!");
#endif
                        if (Util.RestrictionCheckNone(__instance, "damagemultipliertomobs") && (__instance.m_faction != Character.Faction.Players))
                        {
                            float multiplier = Util.RestrictionCheckFloatReturnNone(__instance, "damagemultipliertomobs");
#if DEBUG
                            Debug.Log($"Multiplier: {multiplier}");
#endif
                            hit.m_damage.m_damage *= multiplier;
                            hit.m_damage.m_blunt *= multiplier;
                            hit.m_damage.m_slash *= multiplier;
                            hit.m_damage.m_pierce *= multiplier;
                            hit.m_damage.m_chop *= multiplier;
                            hit.m_damage.m_pickaxe *= multiplier;
                            hit.m_damage.m_fire *= multiplier;
                            hit.m_damage.m_frost *= multiplier;
                            hit.m_damage.m_lightning *= multiplier;
                            hit.m_damage.m_poison *= multiplier;
                            hit.m_damage.m_spirit *= multiplier;
                            return;
                        }
                    }
                    else if (__instance.m_faction == Character.Faction.Players)
                    {
#if DEBUG
                        Debug.Log("A remote player is being targeted.");
#endif
                        if (Util.RestrictionCheckCharacter(__instance, "damagemultipliertoplayers"))
                        {
                            float multiplier = Util.RestrictionCheckFloatReturnCharacter(__instance, "damagemultipliertoplayers");
#if DEBUG
                            Debug.Log($"Mutliplier: {multiplier}");
#endif
                            hit.m_damage.m_damage *= multiplier;
                            hit.m_damage.m_blunt *= multiplier;
                            hit.m_damage.m_slash *= multiplier;
                            hit.m_damage.m_pierce *= multiplier;
                            hit.m_damage.m_chop *= multiplier;
                            hit.m_damage.m_pickaxe *= multiplier;
                            hit.m_damage.m_fire *= multiplier;
                            hit.m_damage.m_frost *= multiplier;
                            hit.m_damage.m_lightning *= multiplier;
                            hit.m_damage.m_poison *= multiplier;
                            hit.m_damage.m_spirit *= multiplier;
                            return;
                        }
                    }
                }
            }
        }
        // Damage Multipliers! (DamageMultiplierToTrees(1))
        [HarmonyPatch(typeof(TreeBase), "Damage")]
        public static class TreeBase_Modifier
        {
            public static void Prefix(TreeBase __instance, HitData hit)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return;
                }
                if (Util.RestrictionCheck("damagemultipliertotrees"))
                {
                    float multiplier = Util.RestrictionCheckFloatReturn("damagemultipliertotrees");
                    hit.m_damage.m_damage *= multiplier;
                    hit.m_damage.m_blunt *= multiplier;
                    hit.m_damage.m_slash *= multiplier;
                    hit.m_damage.m_pierce *= multiplier;
                    hit.m_damage.m_chop *= multiplier;
                    hit.m_damage.m_pickaxe *= multiplier;
                    hit.m_damage.m_fire *= multiplier;
                    hit.m_damage.m_frost *= multiplier;
                    hit.m_damage.m_lightning *= multiplier;
                    hit.m_damage.m_poison *= multiplier;
                    hit.m_damage.m_spirit *= multiplier;
                }
            }
        }
        // Damage Multipliers! (DamageMultiplierToTrees(1))
        [HarmonyPatch(typeof(TreeLog), "Damage")]
        public static class TreeLog_Modifier
        {
            public static void Prefix(TreeLog __instance, HitData hit)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return;
                }
                if (Util.RestrictionCheck("damagemultipliertotrees"))
                {
                    float multiplier = Util.RestrictionCheckFloatReturn("damagemultipliertotrees");
                    hit.m_damage.m_damage *= multiplier;
                    hit.m_damage.m_blunt *= multiplier;
                    hit.m_damage.m_slash *= multiplier;
                    hit.m_damage.m_pierce *= multiplier;
                    hit.m_damage.m_chop *= multiplier;
                    hit.m_damage.m_pickaxe *= multiplier;
                    hit.m_damage.m_fire *= multiplier;
                    hit.m_damage.m_frost *= multiplier;
                    hit.m_damage.m_lightning *= multiplier;
                    hit.m_damage.m_poison *= multiplier;
                    hit.m_damage.m_spirit *= multiplier;
                }
            }
        }


        // No Dropping Items! (NoItemDrop)
        [HarmonyPatch(typeof(InventoryGui), "OnDropOutside")]
        public static class NoDrop_Patch
        {
            private static bool Prefix(InventoryGui __instance)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
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

        // No Dropping Items! (NoItemDrop)
        [HarmonyPatch(typeof(InventoryGrid), "OnLeftClick")]
        public static class NoDrop_Patch2
        {
            private static bool Prefix(InventoryGrid __instance)
            {
                if (WorldofValheimZones.ServerMode)
                {
                    return true;
                }
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
