using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LiteDB;
using System.IO;


//using WorldofValheimServerSideCharacters.Console;


namespace ValheimPermissions
{

    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class ValheimPermissions : BaseUnityPlugin
    {
        public bool runConsole = true;
        public const string Name = ModInfo.Name;
        public const string Guid = ModInfo.Guid;
        public const string Version = ModInfo.Version;

        public static ConfigEntry<string> LiteDB_Location;


        public static ConfigEntry<int> NexusID;
        public static bool ServerMode = Util.IsServer();

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
            ValheimPermissions.NexusID = base.Config.Bind<int>("ValheimPermissions", "NexusID", ModInfo.NexusID, "Nexus ID to make Nexus Update Happy!");
            if (ServerMode)
            {
                Debug.Log("[Server Mode]");
                string testpath = BepInEx.Paths.ConfigPath;
                testpath = Path.Combine(testpath, "WoV");
                ValheimPermissions.LiteDB_Location = base.Config.Bind<string>("ValheimPermissions", "LiteDB_Location", Path.Combine(testpath, "ValheimPermissions.db"), "Where do we want to store the ValheimPermissions.db file at. NOTE: This is relative to the server root path location so if you want it in bepinex config folder do like BepInEx/Config/FILE.DB. Defaults to server root");
                // Load Paths
                ValheimDB.DatabaseLocation = ValheimPermissions.LiteDB_Location.Value;
                if (!Directory.Exists(Path.GetDirectoryName(ValheimDB.DatabaseLocation)))
                    Directory.CreateDirectory(Path.GetDirectoryName(ValheimDB.DatabaseLocation));
                // Run the grand patch all and hope everything works (This is fine...)
            }
            else
            {
                Debug.Log("[Client Mode]");

                // Leave the client state configuration default (Will grab from the server)
            }
            new Harmony(ModInfo.Guid).PatchAll();
        }
        public async void Start()
        {

            await System.Threading.Tasks.Task.Run(() =>
            {
                if (ServerMode)
                {
                    bool RunCommand = true;
                    while (RunCommand)
                    {
                        string input = "";
                        try
                        {
                            input = System.Console.ReadLine();
                            Dedicated_Server.ProcessServerSideCommand(input);
                            input = "";
                        }
                        catch
                        {
                            if (!input.Trim().IsNullOrWhiteSpace())
                            {
                            }
                        }
                    }
                }

            });
        }
    }
}

