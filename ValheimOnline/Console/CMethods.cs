using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using static ValheimOnline.Console.CUtils;

namespace ValheimOnline.Console
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
        public static bool Shutdown(string[] args)
        {
            SkipArg(args);
            Print($"Shutting down the server");
            Util.ServerShutdown();
            return true;
        }

        public static bool SaveAll(string[] args)
        {
            SkipArg(args);
            Print($"Saving all characters");
            Util.SaveAll();
            return true;
        }

        public static bool ZoneReload(string[] args)
        {
            SkipArg(args);
            Print($"Reloading Zones");
            ZoneHandler.LoadZoneData(ValheimOnline.ServerZonePath.Value);

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
