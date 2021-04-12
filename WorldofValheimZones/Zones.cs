using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

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

        public static ConfigEntry<int> NexusID;
        

        // Apparently if this is called in a async then we crash. So there is a variable dedicated to check if we are the server.
        public static bool ServerMode = Util.isServer();


        public void Awake()
        {
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



			if (ServerMode)
			{

				Debug.Log("[Server Mode]");
                string testpath = Config.ConfigFilePath;
                testpath = testpath.Replace("HackShardGaming.WorldofValheimZones.cfg", "WoV");
                WorldofValheimZones.ZonePath = base.Config.Bind<string>("WorldofValheimZones", "ZonePath", Path.Combine(testpath, "zones.txt"), "SERVER ONLY: The file path to the zone file. If it does not exist, it will be created with a default zone.");

                WorldofValheimZones.EnforceZones = base.Config.Bind<bool>("WorldofValheimZones", "EnforceZones", false, "SERVER ONLY: Are we going to enforce zone settings.");
                
                Client.EnforceZones = WorldofValheimZones.EnforceZones.Value;
            }
            

            // Run the grand patch all and hope everything works (This is fine...)
            new Harmony(ModInfo.Guid).PatchAll();


            // Process through the server data needed
            if (ServerMode)
            {
                Debug.Log("[Server Mode]");

                /*
                 * Setup safe zones.
                 */
                ZoneHandler.LoadZoneData(WorldofValheimZones.ZonePath.Value);

            }
        }

#if client_cli

        private static Runner console;
        public bool runConsole = true;

        public async void Start()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                /*
                while ((ZNet.instance == null))
                {
                    Thread.Sleep(1000);// waiting for zNet to initialize
                }*/

                //
                // Setup and start the console
                //

                console = new Runner();

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
                            Console.CUtils.Print($"Please don't use {input} as its causing error," +
                                                $" please report with the dev team, please include input command used");
                        }
                    }
                }
            });
        }
#endif //client_cli

    }
}
