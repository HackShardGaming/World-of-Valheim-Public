using HarmonyLib;

namespace WorldofValheimZones
{
    //Note this is client side only console commands.
    [HarmonyPatch(typeof(Terminal), "InputText")]
    static class F5Console
    {
        static bool Prefix(Terminal __instance)
        {
            string text = __instance.m_input.text;

            // AM I a Player?
            if (Player.m_localPlayer != null)
            {
                // Version results.
                if (text.ToLower().Equals($"!version"))
                {

                    Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: {ModInfo.Version}" }).GetValue();
                    return false;
                }
                // Get my current Coords
                if (text.ToLower().StartsWith($"!getcoords"))
                {
                    CMethods.GetCoords(__instance);
                    return false;
                }
                // Reload zones **ADMIN ONLY**
                if (text.ToLower().StartsWith($"!reload-zones"))
                {
                    CMethods.ReloadZones();
                    return false;
                }
                // Help!
                if (text.ToLower().StartsWith($"!help"))
                {
                    CMethods.Help(__instance);
                    return false;
                }
                // Add Zone **ADMIN ONLY**
                if (text.ToLower().StartsWith($"!addzone"))
                {
                    CMethods.AddZone(__instance, text);
                    return false;
                }
                if (text.ToLower().StartsWith($"!changepvp"))
                {
                    string[] results = text.Split(' ');
                    bool PVPMode = false;
                    if (results[1] == "on")
                    {
                        PVPMode = true;
                    }
                    if (results[1] == "off")
                    {
                        PVPMode = false;
                    }
                    Traverse.Create(__instance).Method("AddString", new object[] { $"Forecfully changing your PVP mode to {results[1]}" }).GetValue();
                    Player.m_localPlayer.SetPVP(PVPMode);
                    return false;
                }
                // Del Zone
                if (text.ToLower().StartsWith($"!delzone"))
                {
                    /*
                    Util.GetServer().rpc.Invoke("delzone", new object[]
                    {
                    });
                    */
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Error: Proper Formating is !delzone [Name]" }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Clinet->Server DelZone" }).GetValue();
                    return false;

                }
                return true;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Chat), "InputText")]
    static class ChatWindow
    {
        static bool Prefix(Terminal __instance)
        {
            string text = __instance.m_input.text;

            // AM I a Player?
            if (Player.m_localPlayer != null)
            {
                // Version results.
                if (text.ToLower().Equals($"!version"))
                {

                    Traverse.Create(__instance).Method("AddString", new object[] { $"{ModInfo.Title}: {ModInfo.Version}" }).GetValue();
                    return false;
                }
                // Get my current Coords
                if (text.ToLower().StartsWith($"!getcoords"))
                {
                    CMethods.GetCoords(__instance);
                    return false;
                }
                // Reload zones **ADMIN ONLY**
                if (text.ToLower().StartsWith($"!reload-zones"))
                {
                    CMethods.ReloadZones();
                    return false;
                }
                // Help!
                if (text.ToLower().StartsWith($"!help"))
                {
                    CMethods.Help(__instance);
                    return false;
                }
                // Add Zone **ADMIN ONLY**
                if (text.ToLower().StartsWith($"!addzone"))
                {
                    CMethods.AddZone(__instance, text);
                    return false;
                }
                if (text.ToLower().StartsWith($"!changepvp"))
                {
                    string[] results = text.Split(' ');
                    bool PVPMode = false;
                    if (results[1] == "on")
                    {
                        PVPMode = true;
                    }
                    if (results[1] == "off")
                    {
                        PVPMode = false;
                    }
                    Traverse.Create(__instance).Method("AddString", new object[] { $"Forecfully changing your PVP mode to {results[1]}" }).GetValue();
                    Player.m_localPlayer.SetPVP(PVPMode);
                    return false;
                }
                // Del Zone
                if (text.ToLower().StartsWith($"!delzone"))
                {
                    /*
                    Util.GetServer().rpc.Invoke("delzone", new object[]
                    {
                    });
                    */
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Error: Proper Formating is !delzone [Name]" }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Clinet->Server DelZone" }).GetValue();
                    return false;

                }
                return true;
            }
            return true;
        }
    }
#if clientcli
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
#endif

}
