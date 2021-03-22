using HarmonyLib;

namespace ValheimOnline
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
			return DisableSave.m_allow_save;
		}

		public static bool m_allow_save;
	}
}
