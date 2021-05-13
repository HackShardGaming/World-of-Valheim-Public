using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
//using WorldofValheimServerSideCharacters.Console;


namespace WorldofValheimServerSideCharacters
{

    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class WorldofValheimServerSideCharacters : BaseUnityPlugin
    {
        public const string Name = ModInfo.Name;
        public const string Guid = ModInfo.Guid;
        public const string Version = ModInfo.Version;
        public static ConfigEntry<string> CharacterSavePath;
        public static ConfigEntry<string> DefaultCharacterPath;
        public static ConfigEntry<int> SaveInterval;
		public static ConfigEntry<int> NexusID;
        public static ConfigEntry<bool> ExportCharacter;
        public static ConfigEntry<bool> AllowMultipleCharacters;
        public static ConfigEntry<int> ShutdownDelay;
        public static ConfigEntry<int> MaxBackups;
        public static ConfigEntry<int> BackupInterval;
        public static bool ServerMode = Util.isServer();

        //private static Console.Console console;
        public bool runConsole = true;

        public void Awake()
        {
#if DEBUG
            Debug.Log("Development Version Activated!!!");
            Debug.Log("Warning: This may break your game (90% stable)");
            Debug.Log("***Do Not Release To Public***");
#endif
            // Process through the configurations
            // Nexus ID For Nexus Update
            WorldofValheimServerSideCharacters.NexusID = base.Config.Bind<int>("WorldofValheimServerSideCharacters", "NexusID", ModInfo.NexusID, "Nexus ID to make Nexus Update Happy!");
			if (ServerMode)
			{
                string testpath = BepInEx.Paths.ConfigPath;
                testpath = Path.Combine(testpath, "WoV");
                WorldofValheimServerSideCharacters.AllowMultipleCharacters = base.Config.Bind<bool>("WorldofValheimServerSideCharacters", "AllowMultipleCharacters", true, "SERVER ONLY: Should we create a new character file if the client logs in using a different character name (TRUE) or should we use only ONE character file per steamid (FALSE)");
                WorldofValheimServerSideCharacters.CharacterSavePath = base.Config.Bind<string>("WorldofValheimServerSideCharacters", "CharacterSavePath", Path.Combine(testpath, "characters"), "SERVER ONLY: The root directory for the server vault.");
                WorldofValheimServerSideCharacters.DefaultCharacterPath = base.Config.Bind<string>("WorldofValheimServerSideCharacters", "DefaultCharacterPath", Path.Combine(testpath, "default_character.fch"), "SERVER ONLY: The file path to the default character file. If it does not exist, it will be created with a default character file.");
                WorldofValheimServerSideCharacters.SaveInterval = base.Config.Bind<int>("WorldofValheimServerSideCharacters", "SaveInterval", 120, "SERVER ONLY: How often, in seconds, to save a copy of each character. Too low may result in performance issues. Too high may result in lost data in the event of a server crash.");
                WorldofValheimServerSideCharacters.ShutdownDelay = base.Config.Bind<int>("WorldofValheimServerSideCharacters", "ShutdownDelay", 15, "SERVER ONLY: How long should we delay after !shutdown has been typed before actually shutting down.");
                WorldofValheimServerSideCharacters.MaxBackups = base.Config.Bind<int>("Backups", "MaxBackups", 5, "SERVER ONLY: How many backups maximum would you like to store on the server? (Default is: 5) (Set to 0 to disable backups!)");
                WorldofValheimServerSideCharacters.BackupInterval = base.Config.Bind<int>("Backups", "BackupInterval", 30, "SERVER ONLY: How often (in minutes) should we make a backup of all characters? (Default is 30 minutes)");
                Util.LoadOrMakeDefaultCharacter();
                Debug.Log("Backup: Creating Backup Thread!");
                Character_Backup.BackupCharacter = new Thread(Character_Backup.BackupScanner);
                if (!Directory.Exists(Path.GetDirectoryName(WorldofValheimServerSideCharacters.CharacterSavePath.Value)))
                    Directory.CreateDirectory(Path.GetDirectoryName(WorldofValheimServerSideCharacters.CharacterSavePath.Value));
                if (!Directory.Exists(Path.GetDirectoryName(WorldofValheimServerSideCharacters.DefaultCharacterPath.Value)))
                    Directory.CreateDirectory(Path.GetDirectoryName(WorldofValheimServerSideCharacters.DefaultCharacterPath.Value));
            }
            else
            {
                Debug.Log("[Client Mode]");
                WorldofValheimServerSideCharacters.ExportCharacter = base.Config.Bind<bool>("WorldofValheimServerSideCharacters", "ExportCharacter", false, "CLIENT ONLY: Export character from server for single player use and/or retain character. Previously AllowCharacterSave (WARNING: THIS WILL OVERWRITE YOUR LOCAL CHARACTER FILE!! PLEASE USE A BLANK CHARACTER FILE!)");
                // Leave the client state configuration default (Will grab from the server)
            }

            // Run the grand patch all and hope everything works (This is fine...)
            new Harmony(ModInfo.Guid).PatchAll();
            Debug.Log("Haz awoke!!?!");
        }
    }
}
