using System.IO;
using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ServerSideCharacters
{

    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class ServerSideCharacters : BaseUnityPlugin
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

        public void Awake()
        {
#if DEBUG
            Debug.Log("Development Version Activated!!!");
            Debug.Log("***Do Not Release To Public***");
#endif
            // Process through the configurations
            // Nexus ID For Nexus Update
            ServerSideCharacters.NexusID = base.Config.Bind<int>(Name, "NexusID", ModInfo.NexusID, "Nexus ID to make Nexus Update Happy!");
			if (ServerMode)
			{
                string NameServer = $"{Name}Server";
                string ConfigPath = BepInEx.Paths.ConfigPath;
                ServerSideCharacters.AllowMultipleCharacters = base.Config.Bind<bool>(NameServer, "AllowMultipleCharacters", true, "SERVER ONLY: Should we create a new character file if the client logs in using a different character name (TRUE) or should we use only ONE character file per steamid (FALSE)");
                ServerSideCharacters.CharacterSavePath = base.Config.Bind<string>(NameServer, "CharacterSavePath", Path.Combine(ConfigPath, "characters"), "SERVER ONLY: The root directory for the server vault.");
                ServerSideCharacters.DefaultCharacterPath = base.Config.Bind<string>(NameServer, "DefaultCharacterPath", Path.Combine(ConfigPath, "default_character.fch"), "SERVER ONLY: The file path to the default character file. If it does not exist, it will be created with a default character file.");
                ServerSideCharacters.SaveInterval = base.Config.Bind<int>(NameServer, "SaveInterval", 120, "SERVER ONLY: How often, in seconds, to save a copy of each character. Too low may result in performance issues. Too high may result in lost data in the event of a server crash.");
                ServerSideCharacters.ShutdownDelay = base.Config.Bind<int>(NameServer, "ShutdownDelay", 15, "SERVER ONLY: How long should we delay after !shutdown has been typed before actually shutting down.");
                ServerSideCharacters.MaxBackups = base.Config.Bind<int>($"{NameServer}Backups", "MaxBackups", 5, "SERVER ONLY: How many backups maximum would you like to store on the server? (Default is: 5) (Set to 0 to disable backups!)");
                ServerSideCharacters.BackupInterval = base.Config.Bind<int>($"{NameServer}Backups", "BackupInterval", 30, "SERVER ONLY: How often (in minutes) should we make a backup of all characters? (Default is 30 minutes)");
                Util.LoadOrMakeDefaultCharacter();
                Debug.Log("Backup: Creating Backup Thread!");
                Character_Backup.BackupCharacter = new Thread(Character_Backup.BackupScanner);
                if (!Directory.Exists(Path.GetDirectoryName(ServerSideCharacters.CharacterSavePath.Value)))
                    Directory.CreateDirectory(Path.GetDirectoryName(ServerSideCharacters.CharacterSavePath.Value));
                if (!Directory.Exists(Path.GetDirectoryName(ServerSideCharacters.DefaultCharacterPath.Value)))
                    Directory.CreateDirectory(Path.GetDirectoryName(ServerSideCharacters.DefaultCharacterPath.Value));
            }
            else
            {
                ServerSideCharacters.ExportCharacter = base.Config.Bind<bool>($"{Name}Client", "ExportCharacter", false, "CLIENT ONLY: Export character from server for single player use and/or retain character. Previously AllowCharacterSave (WARNING: THIS WILL OVERWRITE YOUR LOCAL CHARACTER FILE!! PLEASE USE A BLANK CHARACTER FILE!)");
                // Leave the client state configuration default (Will grab from the server)
            }

            // Run the grand patch all and hope everything works (This is fine...)
            new Harmony(ModInfo.Guid).PatchAll();
            Debug.Log("Haz awoke!");
        }
    }
}
