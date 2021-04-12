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
        public static ConfigEntry<bool> AllowSinglePlayer;
        public static ConfigEntry<int> ShutdownDelay;
        public static bool ServerMode = Util.isServer();

        //private static Console.Console console;
        public bool runConsole = true;

        public void Awake()
        {
			Debug.Log("Haz awoke!!?!");

#if DEBUG
            Debug.Log("Development Version Activated!!!");
            Debug.Log("Warning: This may break your game (90% stable)");
            Debug.Log("***Do Not Release To Public***");
#endif


            // Process through the configurations

            // Nexus ID For Nexus Update
            WorldofValheimServerSideCharacters.NexusID = base.Config.Bind<int>("WorldofValheimServerSideCharacters", "NexusID", 892, "Nexus ID to make Nexus Update Happy!");



			if (ServerMode)
			{
				Debug.Log("[Server Mode]");
                // Load Paths
                string testpath = Config.ConfigFilePath;
                testpath = testpath.Replace("HackShardGaming.WorldofValheimServerSideCharacters.cfg", "WoV");
                WorldofValheimServerSideCharacters.CharacterSavePath = base.Config.Bind<string>("WorldofValheimServerSideCharacters", "CharacterSavePath", Path.Combine(testpath, "characters"), "SERVER ONLY: The root directory for the server vault.");
                WorldofValheimServerSideCharacters.DefaultCharacterPath = base.Config.Bind<string>("WorldofValheimServerSideCharacters", "DefaultCharacterPath", Path.Combine(testpath, "default_character.fch"), "SERVER ONLY: The file path to the default character file. If it does not exist, it will be created with a default character file.");
                WorldofValheimServerSideCharacters.SaveInterval = base.Config.Bind<int>("WorldofValheimServerSideCharacters", "SaveInterval", 120, "SERVER ONLY: How often, in seconds, to save a copy of each character. Too low may result in performance issues. Too high may result in lost data in the event of a server crash.");
                WorldofValheimServerSideCharacters.ShutdownDelay = base.Config.Bind<int>("WorldofValheimServerSideCharacters", "ShutdownDelay", 15, "SERVER ONLY: How long should we delay after !shutdown has been typed before actually shutting down.");
            }
			else
            {
                Debug.Log("[Client Mode]");
                WorldofValheimServerSideCharacters.ExportCharacter = base.Config.Bind<bool>("WorldofValheimServerSideCharacters", "ExportCharacter", false, "CLIENT ONLY: Export character from server for single player use and/or retain character. Previously AllowCharacterSave (WARNING: THIS WILL OVERWRITE YOUR LOCAL CHARACTER FILE!! PLEASE USE A BLANK CHARACTER FILE!)");
                WorldofValheimServerSideCharacters.AllowSinglePlayer = base.Config.Bind<bool>("WorldofValheimServerSideCharacters", "AllowSinglePlayer", false, "CLIENT ONLY: Should we allow the client to play Single Player?  (WARNING: LOTS OF CONSOLE ERRORS RIGHT NOW BUT WORKS!)");
                // Leave the client state configuration default (Will grab from the server)
            }

            // Run the grand patch all and hope everything works (This is fine...)
            new Harmony(ModInfo.Guid).PatchAll();

            // Process through the server data needed
            if (ServerMode)
            {
                Debug.Log("[Server Mode]");

                /*
                 * Setup default character file for server to use.
                 */
                if (!File.Exists(WorldofValheimServerSideCharacters.DefaultCharacterPath.Value))
                {
                    Debug.Log($"Creating default character file at {WorldofValheimServerSideCharacters.DefaultCharacterPath.Value}");
                    Debug.Log("That character does not exist! Loading them up a fresh default!");
                    Directory.CreateDirectory(Path.GetDirectoryName(WorldofValheimServerSideCharacters.DefaultCharacterPath.Value));
                    File.WriteAllBytes(WorldofValheimServerSideCharacters.DefaultCharacterPath.Value,
                        global::WorldofValheimServerSideCharacters.Properties.Resources._default_character);
                }
                else
                {
                    Debug.Log($"Loading default character file from {WorldofValheimServerSideCharacters.DefaultCharacterPath.Value}");
                    ServerState.default_character = File.ReadAllBytes(WorldofValheimServerSideCharacters.DefaultCharacterPath.Value);
                }

                Debug.Log($"Loaded default character file (Size: {ServerState.default_character.Length})");


            }
        }
        /* Disabled until Fixed
        public async void Start()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                console = new Console.Console();
                while (runConsole)
                {
                    string input = "";
                    try
                    {
                        input = System.Console.ReadLine();
                        console.RunCommand(input, false);
                        input = "";
                    }
                    catch
                    {
                        if (!input.Trim().IsNullOrWhiteSpace())
                        {
                            Debug.Log($"Please don't use {input} as its causing error," +
                                                $" please report with the dev team, please include input command used");
                        }
                    }
                }
            });
        }
        */
    }
}
