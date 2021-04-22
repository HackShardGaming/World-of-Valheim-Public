using HarmonyLib;
using UnityEngine.UI;
using System;


namespace WorldofValheimZones
{

    [HarmonyPatch]
    public static class PVPHandler
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game), "Update")]
        private static void Game__Update()
        {
            if (Player.m_localPlayer)
            {
                // 
                // Goes through the zones and setup the necessary enforcements.

                ZoneHandler.Zone zone;
                ZoneHandler.ZoneTypes ztype;
                bool changed;
                bool zonedDetected = ZoneHandler.Detect(Player.m_localPlayer.transform.position, out changed, out zone, out ztype);
                if (changed)
                {
                    if (zonedDetected)
                    {
                        var color = (ztype.PVPEnforce ? (ztype.PVP ? WorldofValheimZones.PVPColor.Value : WorldofValheimZones.PVEColor.Value) : WorldofValheimZones.NonEnforcedColor.Value);
                        string Name = zone.Name.Replace("_", " ");
                        string Message = $"<color={color}>Now entering <b>{Name}</b>.</color>";
                        string BiomeMessage = (ztype.PVPEnforce ? ztype.PVP ? "PVP Enabled" : "PVP Disabled" : String.Empty);
                        // The message at the end is in the format of (PVP) (NOPVP) (NON-ENFORCED)
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, Message,
                                0, null);
                        if (Client.EnforceZones && ztype.PVPEnforce && (ztype.PVP != Player.m_localPlayer.m_pvp))
                            MessageHud.instance.ShowBiomeFoundMsg(BiomeMessage, true);
                    }
                    else
                    {
                        var color = (ztype.PVPEnforce ? (ztype.PVP ? WorldofValheimZones.PVPColor.Value : WorldofValheimZones.PVEColor.Value) : WorldofValheimZones.NonEnforcedColor.Value);
                        string Name = "The Wilderness";
                        string Message = $"<color={color}>Now entering <b>{Name}</b>.</color>";
                        string BiomeMessage = (ztype.PVPEnforce ? ztype.PVP ? "PVP Enabled" : "PVP Disabled" : String.Empty);
                        // The message at the end is in the format of (PVP) (NOPVP) (NON-ENFORCED)
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, Message,
                                0, null);
                        if (Client.EnforceZones && ztype.PVPEnforce && (ztype.PVP != Player.m_localPlayer.m_pvp))
                            MessageHud.instance.ShowBiomeFoundMsg(BiomeMessage, true);

                    }

                    // Zones are now being enforced?
                    if (Client.EnforceZones)
                    {
                        // Update the client settings based on zone type

                        // PVP settings:

                        Client.PVPEnforced = ztype.PVPEnforce;
                        if (ztype.PVPEnforce)
                            Client.PVPMode = ztype.PVP;

                        // Position settings:
                        Client.PositionEnforce = ztype.PositionEnforce;
                        if (ztype.PositionEnforce)
                            Client.ShowPosition = ztype.ShowPosition;
                        // Run the updated settings for the Clients
                        Player.m_localPlayer.SetPVP(Client.PVPMode);
                        InventoryGui.instance.m_pvp.isOn = Client.PVPMode;
                        InventoryGui.instance.m_pvp.interactable = !Client.PVPEnforced;
                        ZNet.instance.SetPublicReferencePosition(Client.ShowPosition);

                        // Other settings are scattered among the wind to other functions
                        // (Use Client class for the current state)
                    }
#if DEBUG
                    ZoneHandler._debug(ztype);
                    Client._debug();
#endif
                }
                else
                {
                    if (Client.PVPEnforced && (Player.m_localPlayer.m_pvp != Client.PVPMode))
                    {
                        Debug.Log($"{ModInfo.Title}: ERROR: Your PVP Mode was changed by another plugin.  Resetting client PVP!");
                        Player.m_localPlayer.SetPVP(Client.PVPMode);
                    }
                    if (Client.PositionEnforce && (ZNet.instance.m_publicReferencePosition != Client.ShowPosition))
                    {
                        Debug.Log($"{ModInfo.Title}: ERROR: Your Position Sharing was changed by another plugin.  Resetting client Position Sharing!");
                        ZNet.instance.SetPublicReferencePosition(Client.ShowPosition);
                    }
                }
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Minimap), "Update")]
        public static void Minimap_Start(Toggle ___m_publicPosition)
        {
            // PositionEnforce : True -> Disable intractable

            ___m_publicPosition.interactable = !Client.PositionEnforce;
        }
        [HarmonyPatch(typeof(InventoryGui), "UpdateCharacterStats")]
        public static class PVP_Patch
        {
            private static void Postfix(InventoryGui __instance)
            {
                __instance.m_pvp.interactable = !Client.PVPEnforced;
            }
        }
    }
}
