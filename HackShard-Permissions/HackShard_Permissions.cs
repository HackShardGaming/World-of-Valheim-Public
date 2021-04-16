using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LiteDB;

//using WorldofValheimServerSideCharacters.Console;


namespace ValheimPermissions
{
    
    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class WorldofValheimServerSideCharacters : BaseUnityPlugin
    {
        public bool runConsole = true;
        public const string Name = ModInfo.Name;
        public const string Guid = ModInfo.Guid;
        public const string Version = ModInfo.Version;

        public static ConfigEntry<string> SQL_HostName;
        public static ConfigEntry<int> SQL_Port;
        public static ConfigEntry<string> SQL_Username;
        public static ConfigEntry<string> SQL_Password;
        public static ConfigEntry<string> SQL_Database;


        public static ConfigEntry<int> NexusID;
        public static bool ServerMode = Util.isServer();

        //private static Console.Console console;

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
            WorldofValheimServerSideCharacters.NexusID = base.Config.Bind<int>("WorldofValheimServerSideCharacters", "NexusID", ModInfo.NexusID, "Nexus ID to make Nexus Update Happy!");
            if (ServerMode)
            {
                Debug.Log("[Server Mode]");
                // Load Paths
            }
            else
            {
                Debug.Log("[Client Mode]");
               // Leave the client state configuration default (Will grab from the server)
            }

            // Run the grand patch all and hope everything works (This is fine...)
            new Harmony(ModInfo.Guid).PatchAll();

        }
        public async void Start()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                bool RunCommand = true;
                while (RunCommand)
                {
                    string input = "";
                    try
                    {
                        input = System.Console.ReadLine();
                        Dedicated_Server.RunCommand(input);
                        input = "";
                    }
                    catch
                    {
                        if (!input.Trim().IsNullOrWhiteSpace())
                        {
                        }
                    }
                }
            });
        }
    }
}
