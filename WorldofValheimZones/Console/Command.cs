using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;

namespace WorldofValheimZones
{
    //Note this is client side only console commands.
    [HarmonyPatch(typeof(Console), "InputText")]
    static class InputText_Patch
    {

        static bool Prefix(Console __instance)
        {
            string text = __instance.m_input.text;
            
            // AM I a Player?
            if (Player.m_localPlayer != null)
            {
                // Version results.
                if (text.ToLower().Equals($"!version"))
                {

                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones Version: {ModInfo.Version}" }).GetValue();
                    return false;
                }
                // Add Zone
                if (text.ToLower().StartsWith($"!addzone"))
                {
                    string[] results = text.Split(' ');
                    if (results.Count() == 8 && results[4].ToLower() == "circle" || results[4].ToLower() == "square")
                    {
                        Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                        Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Client->Server AddZone!" }).GetValue();
                        /*
                        Util.GetServer().rpc.Invoke("addzone", new object[]
                        {
                        });
                        */
                        return false;
                    }
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-Zones: Error in formating. The proper formating is !addzone [Name] [ZoneType] [Priority] [Shape(circle/square)] [x] [y] [r]" }).GetValue();
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
}
#if client_cli

namespace WorldofValheimZones.Console
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
#endif
