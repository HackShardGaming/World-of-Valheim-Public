using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace WorldofValheimServerSideCharacters
{
    //Note this is client side only console commands.
    [HarmonyPatch(typeof(Console), "InputText")]
    static class InputText_Patch
    {


        static bool Prefix(Console __instance)
        {

            string text = __instance.m_input.text;
            // Lets check the version!
            if (text.ToLower().Equals($"!version"))
            {
                if (Player.m_localPlayer != null)
                {
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-SSC Version: {ModInfo.Version}" }).GetValue();
                    return false;
                }
                else
                    return true;
            }
            if (text.ToLower().StartsWith($"!help"))
            {
                Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                Traverse.Create(__instance).Method("AddString", new object[] { $"!save (Saves your character (server side))" }).GetValue();
                Traverse.Create(__instance).Method("AddString", new object[] { $"!save-all (Request the server to save all clients **ADMIN COMMAND**)" }).GetValue();
                Traverse.Create(__instance).Method("AddString", new object[] { $"!shutdown-server (Shuts the server down **ADMIN COMMAND**)" }).GetValue();
            }
            if (text.ToLower().Equals($"!save"))
            {
                if (Player.m_localPlayer != null)
                {
                    Util.GetServer().rpc.Invoke("CharacterUpdate", new object[]
                    {
                    Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
                    });
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { $"WoV-SSC: Clinet->Server CharacterUpdate" }).GetValue();
                    return false;
                }
                else
                    return true;
            }
            if (text.ToLower().StartsWith($"!shutdown-server"))
            {
                ZPackage pkg = new ZPackage(); // Create ZPackage
                string msg = "ShutdownServer";
                pkg.Write(msg);
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "ShutdownServer", new object[] { pkg });
                return false;
            }
            if (text.ToLower().StartsWith($"!save-all"))
            {
                ZPackage pkg = new ZPackage(); // Create ZPackage
                string msg = "SaveAll";
                pkg.Write(msg);
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "SaveAll", new object[] { pkg });
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