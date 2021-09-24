using HarmonyLib;

namespace WorldofValheimServerSideCharacters
{
    public static class Dedicated_Server
    {
        public static void RunCommand(string text)
        {
            if (text.ToLower().Equals($"!shutdown-server"))
            {
                Debug.Log($"{ModInfo.Title}: Initiating server shutdown now!");
                Game.instance.StartCoroutine(Util.ShutdownServer());
                return;
            }
            if (text.ToLower().Equals($"!save-all"))
            {
                Debug.Log($"{ModInfo.Title}: Requesting a save from all connected users!");
                Util.SaveAll();
            }
        }
    }

    //Note this is client side only console commands.
    [HarmonyPatch(typeof(Terminal), "InputText")]
    static class InputText_Patch
    {
        static bool Prefix(Terminal __instance)
        {

            string text = __instance.m_input.text;
            // Lets check the version!
            if (text.ToLower().Equals($"!version"))
            {
                if (Player.m_localPlayer != null)
                {
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: {ModInfo.Version}" }).GetValue();
                    return false;
                }
                else
                    return true;
            }
            // Help Menu
            if (text.ToLower().StartsWith($"!help"))
            {
                CMethods.Help(__instance);
                return false;
            }
            // Save my character
            if (text.ToLower().Equals($"!save"))
            {
                CMethods.Save(__instance);
                return false;
            }
            // Shutdown the server **Admin Command**
            if (text.ToLower().StartsWith($"!shutdown-server"))
            {
                CMethods.ShutdownServer();
                return false;
            }
            if (text.ToLower().Equals($"!reloaddefault"))
            {
                Debug.Log($"{ModInfo.Title}: Reload default character file!");
                CMethods.ReloadDefault();
            }
            /*
            if (text.ToLower().StartsWith($"!broadcast"))
            {
                CMethods.Broadcast(__instance);
                return false
            }
            */
            // Save all online users **Admin Command**
            if (text.ToLower().StartsWith($"!save-all"))
            {
                CMethods.SaveAll();
                return false;
            }
            return true;
        }
    }
}

/* Disabling until Fixed
namespace WorldofValheimServerSideCharacters.Console 
{
    // Main command handler class for the plugin
    class Command
    {
        private readonly string commandName;
        private readonly string argHint;
        private readonly bool adminCmd;
        private readonly Method method;

        public Command(string commandName, string argHint, Method method, bool adminCmd)
        {
            this.commandName = commandName;
            this.argHint = argHint;
            this.method = method;
            this.adminCmd = adminCmd;
        }

        public delegate bool Method(string[] args);
        public string Name { get => commandName; }
        public string Hint { get => argHint; }
        public bool AdminCmd { get => adminCmd; }

        public bool Run(string[] args)
        {
            return (bool)method.DynamicInvoke(new object[] { args });
        }

    }
}
*/