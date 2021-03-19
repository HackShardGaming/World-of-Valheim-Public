using System;
using HarmonyLib;

namespace VOnline
{

	[HarmonyPatch]
	public static class Debug
	{

		public static void Assert(bool cond)
		{
		}

		public static void Log(string str)
		{
			System.Console.WriteLine("VONLINE: " + str);
		}
	}
}
