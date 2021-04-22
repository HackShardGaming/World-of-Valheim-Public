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

#if client_cli
using WorldofValheimZones.Console;
#endif

namespace WorldofValheimZones
{
    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class WorldofValheimZones : BaseUnityPlugin
    {
        private static WorldofValheimZones context;

        public const string Name = ModInfo.Name;
        public const string Guid = ModInfo.Guid;
        public const string Version = ModInfo.Version;
        
        public static ConfigEntry<string> ZonePath;
        public static ConfigEntry<bool> EnforceZones;
        public static ConfigEntry<string> PVPColor;
        public static ConfigEntry<string> PVEColor;
        public static ConfigEntry<string> NonEnforcedColor;
        public const int AreaRange = 100;
        public static int HealTick = 0;
        public static int DamageTick = 0;
        public static int EffectTick = 0;
        public static bool HELLOGITHUB = false;
        private string hashCheck = "";
        public static Harmony harm = new Harmony("ZonePermissions");
        private static WorldofValheimZones plugin;
        public static ConfigEntry<string> ZoneConfigurationPath;

        public static ConfigEntry<int> NexusID;
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
            if (SERVER)
            {
                Debug.Log("[Server Mode]");
                WorldofValheimZones.ZonePath = base.Config.Bind<string>("WorldofValheimZones", "ZonePath", ZonesLocation, "SERVER ONLY: The file path to the zone file. If it does not exist, it will be created with a default zone.");
                WorldofValheimZones.EnforceZones = base.Config.Bind<bool>("WorldofValheimZones", "EnforceZones", false, "SERVER ONLY: Are we going to enforce zone settings.");
                Client.EnforceZones = WorldofValheimZones.EnforceZones.Value;
                WorldofValheimZones.ZoneConfigurationPath = base.Config.Bind<string>("WorldofValheimZones", "ZoneConfigurationPath", ZoneConfiguration_Location, "SERVER ONLY: Location of the ZonesPermissions file.");
                string pathwithoutfile2 = Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value);
                if (!Directory.Exists(pathwithoutfile2))
                    Directory.CreateDirectory(Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value));
                Debug.Log(Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value));
                if (!File.Exists(WorldofValheimZones.ZonePath.Value))
                {
                    Debug.Log($"Creating Zones file at {WorldofValheimZones.ZonePath.Value}");
                    string text = global::WorldofValheimZones.Properties.Resources.Default_zones;
                    //string text = "# format: name type x z radius\nDefaultSafeZone safe 1 0.0 0.0 5.0 true";
                    File.WriteAllText(WorldofValheimZones.ZonePath.Value, text);
                }
                string pathwithoutfile = Path.GetDirectoryName(WorldofValheimZones.ZoneConfigurationPath.Value);
                if (!Directory.Exists(pathwithoutfile))
                    Directory.CreateDirectory(Path.GetDirectoryName(WorldofValheimZones.ZoneConfigurationPath.Value));
                Debug.Log(Path.GetDirectoryName(WorldofValheimZones.ZoneConfigurationPath.Value));
                if (!File.Exists(WorldofValheimZones.ZoneConfigurationPath.Value))
                {
                    Debug.Log($"Creating zone permissions file at {WorldofValheimZones.ZoneConfigurationPath.Value}");
                    string text = global::WorldofValheimZones.Properties.Resources.Default_Zone_Configurations;
                    //string text = "# format: name type x z radius\nDefaultSafeZone safe 1 0.0 0.0 5.0 true";
                    File.WriteAllText(WorldofValheimZones.ZoneConfigurationPath.Value, text);
                }
                else
                {
                    hashCheck = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(WorldofValheimZones.ZoneConfigurationPath.Value))).Replace("-", "").ToLower();
                }
                watcher = new FileSystemWatcher(Path.GetDirectoryName(WorldofValheimZones.ZoneConfigurationPath.Value));
                watcher.IncludeSubdirectories = true;
                Debug.Log("STARTED WATCHER AT " + Path.GetDirectoryName(WorldofValheimZones.ZoneConfigurationPath.Value));
                watcher.Changed += OnChangedAREA;
                watcher.Filter = Path.GetFileName(WorldofValheimZones.ZoneConfigurationPath.Value);
                watcher.EnableRaisingEvents = true;

                zonewatcher = new FileSystemWatcher(Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value));
                Debug.Log("STARTED WATCHER AT " + Path.GetDirectoryName(WorldofValheimZones.ZonePath.Value));
                zonewatcher.Changed += OnChangedZONE;
                zonewatcher.Filter = Path.GetFileName(WorldofValheimZones.ZonePath.Value);
                zonewatcher.EnableRaisingEvents = true;
                ZoneHandler.LoadZoneData(WorldofValheimZones.ZonePath.Value);
                ZoneHandler.LoadZoneConfigurationData(WorldofValheimZones.ZoneConfigurationPath.Value);

            }
            else
            {
                Debug.Log("[Client Mode]");
                WorldofValheimZones.PVPColor = base.Config.Bind<string>("Colors", "PVPColor", "Red", "What color should our 'Now Entering' message be if the zone type has PVP on");
                WorldofValheimZones.PVEColor = base.Config.Bind<string>("Colors", "PVEColor", "White", "What color should our 'Now Entering' message be if the zone type has PVE off");
                WorldofValheimZones.NonEnforcedColor = base.Config.Bind<string>("Colors", "NonEnforcedColor", "Yellow", "What color should our 'Now Entering' message be if the zone type has No PVP Enforcement");

            }
            context = this;
            Debug.Log("Haz awoke!!?!");

#if DEBUG
            Debug.Log("Development Version Activated!!!");
            Debug.Log("Warning: This may break your game (90% stable)");
            Debug.Log("***Do Not Release To Public***");
#endif
            // Process through the configurations

            // Nexus ID For Nexus Update
            WorldofValheimZones.NexusID = base.Config.Bind<int>("WorldofValheimZones", "NexusID", ModInfo.NexusID, "Nexus ID to make Nexus Update Happy!");
            

            // Run the grand patch all and hope everything works (This is fine...)
            new Harmony(ModInfo.Guid).PatchAll();
            // Process through the server data needed
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
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ZoneHandler", new object[] {
                        ZoneHandler.Serialize(null)
                    });
        }
        private static FileSystemWatcher watcher;
        private static void OnChangedAREA(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            ZLog.Log("AREA FILE CHANGED");
            if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
            {
                ZoneHandler.LoadZoneConfigurationData(WorldofValheimZones.ZoneConfigurationPath.Value);
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ZoneHandler", new object[] {
                        ZoneHandler.Serialize(null)
                    });
            }
        }
    }
}
