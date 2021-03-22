using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;


namespace ValheimOnline
{

    [HarmonyPatch]
    public static class PVPHandler
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Minimap), "Start")]
        public static void Minimap_Start(Toggle ___m_publicPosition)
        {
            // PVPEnforced : True -> Disable interactable
            ___m_publicPosition.interactable = !ServerState.PVPEnforced;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Player), "SetPVP")]
        public static IEnumerable<CodeInstruction> Transpiler__SetPVP(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Calls(PVPHandler.func_message))
                {
                    list[i].opcode = OpCodes.Call;
                    list[i].operand = PVPHandler.func_messageNone;
                }
            }

            return list.AsEnumerable<CodeInstruction>();

        }

        private static void MessageNone(Character _0, MessageHud.MessageType _1, string _2, int _3, Sprite _4)
        {
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(InventoryGui), "UpdateCharacterStats")]
        public static IEnumerable<CodeInstruction> Transpiler__UpdateCharacterStats(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].LoadsField(PVPHandler.f_m_pvp, !ServerState.PVPEnforced))
                {
                    i--;
                    list.RemoveRange(i, list.Count - i - 1);
                    list.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0, null));
                    list.Insert(i++, new CodeInstruction(OpCodes.Ldarg_1, null));
                    list.Insert(i++, new CodeInstruction(OpCodes.Call, PVPHandler.func_handleInteraction));
                    list.Insert(i++, new CodeInstruction(OpCodes.Ret, null));
                }
            }
            return list.AsEnumerable<CodeInstruction>();
        }

        public static void HandleInteraction(InventoryGui instance, Player player)
        {
            instance.m_pvp.interactable = !ServerState.PVPEnforced;
            instance.m_pvp.isOn = player.IsPVPEnabled();
        }

        private static MethodInfo func_message = AccessTools.Method(typeof(Character), "Message", null, null);

        private static MethodInfo func_messageNone = AccessTools.Method(typeof(PVPHandler), "MessageNone", null, null);

        private static FieldInfo f_m_pvp = AccessTools.Field(typeof(InventoryGui), "m_pvp");

        private static MethodInfo func_handleInteraction = AccessTools.Method(typeof(PVPHandler), "HandleInteraction", null, null);
    }
}
