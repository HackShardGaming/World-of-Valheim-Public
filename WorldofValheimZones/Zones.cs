using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Security.Cryptography;
using Steamworks;
using System.Globalization;
#if client_cli
using WorldofValheimZones.Console;
#endif
namespace WorldofValheimZones
{
    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class WorldofValheimZones : BaseUnityPlugin
    {
        public const string Name = ModInfo.Name;
        public const string Guid = ModInfo.Guid;
        public const string Version = ModInfo.Version;
        public static ConfigEntry<string> ZonePath;
        public static ConfigEntry<string> PVPColor;
        public static ConfigEntry<string> PVEColor;
        public static ConfigEntry<string> NonEnforcedColor;
        public static ConfigEntry<bool> WardProtectItemDrop;
        public static ConfigEntry<bool> WardProtectItemPickup;
        public static ConfigEntry<bool> WardProtectDamage;
        public static ConfigEntry<bool> ReloadDetection;
        public const int AreaRange = 100;
        public static int HealTick = 0;
        public static int DamageTick = 0;
        public static int EffectTick = 0;
        public static Harmony harm = new Harmony("ZonePermissions");
        private static WorldofValheimZones plugin;
        public static ConfigEntry<string> ZoneConfigurationPath;
        public static ConfigEntry<int> NexusID;
        public static ConfigEntry<bool> BiomePVPAnnouncement;
        public static ConfigEntry<bool> NoItemLoss;
        public static ConfigEntry<Single> RespawnTimer;
        public static string MySteamID = "";
        // Apparently if this is called in a async then we crash. So there is a variable dedicated to check if we are the server.
        public static bool ServerMode = Util.isServer();
        void FixedUpdate()
        {
            Patches.FixedUpdatez();
        }
        public void Awake()
        {
            string ConfigPath = Path.Combine(BepInEx.Paths.ConfigPath, "WoV");
            string ZonesLocation = Path.Combine(ConfigPath, "Zones.txt");
            string ZoneConfiguration_Location = Path.Combine(ConfigPath, "Zone_Configuration.txt");
                        plugin = this;
            bool SERVER = Paths.ProcessName.Equals("valheim_server", StringComparison.OrdinalIgnoreCase) ? true : false;
            WorldofValheimZones.NexusID = base.Config.Bind<int>("WorldofValheimZones", "NexusID", ModInfo.NexusID, "Nexus ID to make Nexus Update Happy!");
            Client.EnforceZones = true;
            if (SERVER)
            {
                Debug.Log("[Server Mode]");
                WorldofValheimZones.ReloadDetection = base.Config.Bind<bool>("Config", "ReloadDetection", false, "SERVER ONLY: Should the server auto reload if the config file is changed? (May cause DeSync)");
                WorldofValheimZones.NoItemLoss = base.Config.Bind<bool>("Death", "NoItemLoss", false, "SERVER ONLY: Should we prevent a user from losing items/skills on death globally?");
                WorldofValheimZones.RespawnTimer = base.Config.Bind<Single>("Death", "RespawnTimer", 10, "SERVER ONLY: How fast should the clients respawn?");
                WorldofValheimZones.ZonePath = base.Config.Bind<string>("WorldofValheimZones", "ZonePath", ZonesLocation, "SERVER ONLY: The file path to the zone file. If it does not exist, it will be created with a default zone.");
                WorldofValheimZones.WardProtectDamage = base.Config.Bind<bool>("Ward", "Building_ProtectDamage", false, "SERVER ONLY: Protect buildings from being damaged inside Warded Areas?");
                WorldofValheimZones.WardProtectItemPickup = base.Config.Bind<bool>("Ward", "Item_Pickup", false, "SERVER ONLY: Protect Picking up items in Warded Areas?");
                WorldofValheimZones.WardProtectItemDrop = base.Config.Bind<bool>("Ward", "Item_Drop", false, "SERVER ONLY: Protect Dropping items in Warded Areas?");
                Client.Ward.Damage = WorldofValheimZones.WardProtectDamage.Value;
                Client.Ward.Pickup = WorldofValheimZones.WardProtectItemPickup.Value;
                Client.Ward.Drop = WorldofValheimZones.WardProtectItemDrop.Value;
                Client.NoItemLoss = WorldofValheimZones.NoItemLoss.Value;
                Client.RespawnTimer = WorldofValheimZones.RespawnTimer.Value;
                // Check if the Zones file and folder exist
                string pathwithoutfile2 = Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value);
                if (!Directory.Exists(pathwithoutfile2))
                    Directory.CreateDirectory(Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value));
                Debug.Log(Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value));
                if (!File.Exists(WorldofValheimZones.ZonePath.Value))
                {
                    Debug.Log($"Creating Zones file at {WorldofValheimZones.ZonePath.Value}");
                    string text = global::WorldofValheimZones.Properties.Resources.Default_zones;
                    File.WriteAllText(WorldofValheimZones.ZonePath.Value, text);
                }
                if (WorldofValheimZones.ReloadDetection.Value)
                {
                    zonewatcher = new FileSystemWatcher(Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value));
                    Debug.Log("STARTED WATCHER AT " + Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value));
                    zonewatcher.Changed += OnChangedZONE;
                    zonewatcher.Filter = Path.GetFileName(WorldofValheimZones.ZonePath.Value);
                    zonewatcher.EnableRaisingEvents = true;
                }
                ZoneHandler.LoadZoneData(WorldofValheimZones.ZonePath.Value);
            }
            else
            {
                Debug.Log("[Client Mode]");
                WorldofValheimZones.BiomePVPAnnouncement = base.Config.Bind<bool>("Biome", "BiomePVPAnnouncement", true, "Should we announce changing PVP in a Biome Announcement? true or false");
                WorldofValheimZones.PVPColor = base.Config.Bind<string>("Colors", "PVPColor", "Red", "What color should our 'Now Entering' message be if the zone type has PVP on");
                WorldofValheimZones.PVEColor = base.Config.Bind<string>("Colors", "PVEColor", "White", "What color should our 'Now Entering' message be if the zone type has PVE off");
                WorldofValheimZones.NonEnforcedColor = base.Config.Bind<string>("Colors", "NonEnforcedColor", "Yellow", "What color should our 'Now Entering' message be if the zone type has No PVP Enforcement");
            }
            Debug.Log("Haz awoke!!?!");
#if DEBUG
            Debug.Log("Development Version Activated!!!");
            Debug.Log("Warning: This may break your game (90% stable)");
            Debug.Log("***Do Not Release To Public***");
#endif
             // Run the grand patch all and hope everything works (This is fine...)
            new Harmony(ModInfo.Guid).PatchAll();
        }
        public class AreaInfo
        {
            public string configs;
        }
        static void OnDestroy()
        {
            harm.UnpatchSelf();
        }
        private static FileSystemWatcher zonewatcher;
        private static void OnChangedZONE(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Debug.Log("ZONES FILE CHANGED!");
            ZoneHandler.LoadZoneData(WorldofValheimZones.ZonePath.Value);
            Util.Broadcast("Reloading Zone");
            Game.instance.StartCoroutine(Util.SendAllUpdate());
        }
    }
}
