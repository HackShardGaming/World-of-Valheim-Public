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
            // PVPEnforced : True -> Disable intractable
            ___m_publicPosition.interactable = !Client.PVPEnforced;
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


        // Harmony Transpiler
        // Patch the InventoryGui::UpdateCharacterStats

        /* code section in game (dnSpy results for reference)
        public void UpdateCharacterStats(Player player)
        {
            PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
            this.m_playerName.text = playerProfile.GetName();
            float bodyArmor = player.GetBodyArmor();
            this.m_armor.text = bodyArmor.ToString();
            this.m_pvp.interactable = player.CanSwitchPVP();
            player.SetPVP(this.m_pvp.isOn);
        }*/


        [HarmonyTranspiler]
        [HarmonyPatch(typeof(InventoryGui), "UpdateCharacterStats")]
        public static IEnumerable<CodeInstruction> Transpiler__UpdateCharacterStats(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
            for (int i = 0; i < list.Count; i++)
            {
                // Will go through all the Code Instructions until we find the following handler in the code.
                // Do NOT change the values below.
                // This function just looks for the handler and the by address field is meant to be false.
                if (list[i].LoadsField(PVPHandler.f_m_pvp, false))
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
            instance.m_pvp.interactable = !Client.PVPEnforced;
            if (Client.PVPEnforced == true)
            {
                instance.m_pvp.isOn = Client.PVPMode;
            }
        }

        private static MethodInfo func_message = AccessTools.Method(typeof(Character), "Message", null, null);

        private static MethodInfo func_messageNone = AccessTools.Method(typeof(PVPHandler), "MessageNone", null, null);

        private static FieldInfo f_m_pvp = AccessTools.Field(typeof(InventoryGui), "m_pvp");

        private static MethodInfo func_handleInteraction = AccessTools.Method(typeof(PVPHandler), "HandleInteraction", null, null);
    }
}
