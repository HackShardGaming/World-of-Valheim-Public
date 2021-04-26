using HarmonyLib;

namespace WorldofValheimServerSideCharacters
{
    [HarmonyPatch]
    public static class DisableSave
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FejdStartup), "OnNewCharacterDone")]
        private static void FejdStartup__OnNewCharacterDone_Prefix()
        {
            DisableSave.m_allow_save = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FejdStartup), "OnNewCharacterDone")]
        private static void FejdStartup__OnNewCharacterDone_Postfix()
        {
            DisableSave.m_allow_save = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerProfile), "SavePlayerToDisk")]
        private static bool PlayerProfile__SavePlayerToDisk()
        {
            Debug.Log($"Connections: {ServerState.Connections.Count}");
            // Is there a connection to a server?
            if (ServerState.ConnectionCount > 0)
            {
                // Are we allowed to export our characters?
                if (WorldofValheimServerSideCharacters.ExportCharacter.Value)
                {
                    Debug.Log($"Your WoV-SSC Character file has been locally saved!");
                    return true;
                }
                // if not return false.
                else
                {
                    return false;
                }
            }
            // No connection to a server so lets save the character!
            else
            {
                return true;
            }
        }
        public static bool m_allow_save;
    }
}
