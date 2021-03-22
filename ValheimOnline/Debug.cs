using HarmonyLib;

namespace ValheimOnline
{
	// Debug Patch Class
	[HarmonyPatch]
	public static class Debug
	{

		public static void Assert(bool cond)
		{
		}

		public static void Log(string str)
		{
			System.Console.WriteLine("Valheim Online: " + str);
		}
	}
}
