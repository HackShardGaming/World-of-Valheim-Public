using HarmonyLib;


namespace ValheimPermissions
{
    // Debug Patch Class
    [HarmonyPatch]
    public static class Debug
    {
        // This is the Debug Log.  It will put the ModInfo.Title at the start of the log line and send it directly to the console..
        public static void Log(string str)
        {
            System.Console.WriteLine($"{ModInfo.Title}: " + str);
        }
        // This is the untagged log.  It will just send the string directly to the console.
        public static void Loguntagged(string str)
        {
            System.Console.WriteLine(str);
        }
    }
}
