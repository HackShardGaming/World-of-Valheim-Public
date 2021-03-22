using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ValheimOnline
{

	[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
	public class ValheimOnline : BaseUnityPlugin
    {
        public const string Name = ModInfo.Name;
        public const string Guid = ModInfo.Guid;
		public const string Version = ModInfo.Version;

		public static ConfigEntry<string> ServerVaultPath;
        public static ConfigEntry<string> ServerSafeZonePath;
        public static ConfigEntry<string> ServerBattleZonePath;
		public static ConfigEntry<int> ServerSaveInterval;
        public static ConfigEntry<bool> ServerPvpEnforced;
        public static ConfigEntry<bool> PVPSharePosition;

		public void Awake()
		{
            ValheimOnline.ServerVaultPath = base.Config.Bind<string>("ValheimOnline", "ServerVaultPath", Path.Combine(Utils.GetSaveDataPath(), "characters_vault"), "SERVER ONLY: The root directory for the server vault.");
            ValheimOnline.ServerSafeZonePath = base.Config.Bind<string>("ValheimOnline", "ServerSafeZonePath", Path.Combine(Utils.GetSaveDataPath(), "safe_zones.txt"), "SERVER ONLY: The file path to the safe zone file. If it does not exist, it will be created with a default safe zone.");
            ValheimOnline.ServerBattleZonePath = base.Config.Bind<string>("ValheimOnline", "ServerBattleZonePath", Path.Combine(Utils.GetSaveDataPath(), "Battle_zones.txt"), "SERVER ONLY: The file path to the Battle zone file. If it does not exist, it will be created with a default Battle zone.");
			ValheimOnline.ServerSaveInterval = base.Config.Bind<int>("ValheimOnline", "ServerSaveInterval", 600, "SERVER ONLY: How often, in seconds, to save a copy of each character. Too low may result in performance issues. Too high may result in lost data in the event of a server crash.");
            ValheimOnline.ServerPvpEnforced = base.Config.Bind<bool>("ValheimOnline", "ServerPvpEnforced", false, "SERVER ONLY: Enforce the servers PVP mode and prevent users from changing.");
			ValheimOnline.PVPSharePosition = base.Config.Bind<bool>("ValheimOnline", "PVPSharePosition", true, "SERVER ONLY: Shows the user on the MAP.");
			new Harmony(ModInfo.Guid).PatchAll();

            ServerState.PVPEnforced = ValheimOnline.ServerPvpEnforced.Value;
            ServerState.PVPSharePosition = ValheimOnline.PVPSharePosition.Value;
			/*
			 * Setup safe zones.
			 */

			if (!File.Exists(ValheimOnline.ServerSafeZonePath.Value))
			{
				Debug.Log(string.Format("Creating safe zone file at {0}", ValheimOnline.ServerSafeZonePath.Value));
				string text = "# format: name x z radius\nDefaultSpawnSafeZone 0.0 0.0 50.0";
				File.WriteAllText(ValheimOnline.ServerSafeZonePath.Value, text);
			}
			foreach (string text2 in File.ReadAllLines(ValheimOnline.ServerSafeZonePath.Value))
			{
				if (!string.IsNullOrWhiteSpace(text2) && text2[0] != '#')
				{
					string[] array2 = text2.Split(Array.Empty<char>());
					if (array2.Length != 4)
					{
						Debug.Log(string.Format("Safe zone {0} is not correctly formatted.", text2));
					}
					else
					{
						ServerState.SafeZone safeZone;
						safeZone.name = array2[0];
						safeZone.position.x = float.Parse(array2[1]);
						safeZone.position.y = float.Parse(array2[2]);
						safeZone.radius = float.Parse(array2[3]);
						Debug.Log(string.Format("Loaded safe zone {0} ({1}, {2}) radius {3}", new object[]
						{
							safeZone.name,
							safeZone.position.x,
							safeZone.position.y,
							safeZone.radius
						}));
						ServerState.SafeZones.Add(safeZone);
					}
				}
			}
		}
    }
}
