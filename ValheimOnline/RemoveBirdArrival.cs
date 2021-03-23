using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace ValheimOnline
{

	[HarmonyPatch(typeof(Game), "UpdateRespawn")]
	public static class RemoveBirdArrival
	{

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Calls(RemoveBirdArrival.func_sendText))
				{
					list.RemoveRange(i - 3, 4);
					break;
				}
			}
			return list.AsEnumerable<CodeInstruction>();
		}

		private static MethodInfo func_sendText = AccessTools.Method(typeof(Chat), "SendText", null, null);
	}
}
