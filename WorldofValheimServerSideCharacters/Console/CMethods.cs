using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
/* Disabling Until fixed
using static WorldofValheimServerSideCharacters.Console.CUtils;

namespace WorldofValheimServerSideCharacters.Console
{
    class CMethods
    {
        // Utils
        public static bool Help(string[] args)
        {
            Debug.Log("Commands: ");
            foreach (Command cmd in Console.Instance.commands)
            {
                Debug.Log(cmd.Hint);
            }

            return true;
        }

        public static void SkipArg(string[] args)
        {
            string message = CombineArgs(args);
            if (!message.IsNullOrWhiteSpace())
            {
                Debug.Log($"Unnecessary argument skipped: {message}");
            }
        }

        // CLI Functions
        public static bool PrintVersion(string[] args)
        {
            SkipArg(args);
            Debug.Log($"Valheim Server Side Characters version: {ModInfo.Version}");
            return true;
        }
        public static bool Shutdown(string[] args)
        {
            SkipArg(args);
            Debug.Log($"Shutting down the server");
            Util.ServerShutdown();
            return true;
        }

        public static bool SaveAll(string[] args)
        {
            SkipArg(args);
            Debug.Log($"Saving all characters");
            Util.SaveAll();
            return true;
        }


    }
}
*/