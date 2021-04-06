
#if client_cli

using BepInEx;
using static WorldofValheimZones.Console.CUtils;

namespace WorldofValheimZones.Console
{
    class CMethods
    {
        // Utils
        public static bool Help(string[] args)
        {
            Print("Commands: ");
            foreach (Command cmd in Runner.Instance.commands)
            {
                Print(cmd.Hint);
            }

            return true;
        }

        public static void SkipArg(string[] args)
        {
            string message = CombineArgs(args);
            if (!message.IsNullOrWhiteSpace())
            {
                Print($"Unnecessary argument skipped: {message}");
            }
        }

        // CLI Functions
        public static bool PrintVersion(string[] args)
        {
            SkipArg(args);
            Print($"Valheim Online version: {ModInfo.Version}");
            return true;
        }

        public static bool ZoneReload(string[] args)
        {
            SkipArg(args);
            Print($"Reloading Zones");
            ZoneHandler.LoadZoneData(WorldofValheimZones.ServerZonePath.Value);

            /*
            Util.GetServer().rpc.Invoke("ChatMessage", new object[] {
                new Vector3(), 1, "server", "Reloading Zones"
            });
            */

            Util.Broadcast("Reloading Zone");

            Debug.Log("S2C ZoneHandler (SendPeerInfo)");
            Util.GetServer().rpc.Invoke("ZoneHandler", new object[] {
                ZoneHandler.Serialize()
            });

            /*
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ZoneHandler",new object[] {
                ZoneHandler.Serialize()
            });

            /*
             *
             * rpc.Invoke("ZoneHandler", new object[] {
                ZoneHandler.Serialize()
            });
             */

            return true;
        }

    }
}

#endif