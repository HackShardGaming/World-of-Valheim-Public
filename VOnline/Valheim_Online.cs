using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace VOnline
{

	[BepInPlugin("Aluviel.VOnline", "VOnline", "0.1")]
	public class Valheim_Online : BaseUnityPlugin
	{

		public void Awake()
		{
			new Harmony("Aluviel.VOnline").PatchAll();
			Valheim_Online.ServerVaultPath = base.Config.Bind<string>("VOnline", "ServerVaultPath", Path.Combine(Utils.GetSaveDataPath(), "characters_vault"), "SERVER ONLY: The root directory for the server vault.");
			Valheim_Online.ServerSafeZonePath = base.Config.Bind<string>("VOnline", "ServerSafeZonePath", Path.Combine(Utils.GetSaveDataPath(), "safe_zones.txt"), "SERVER ONLY: The file path to the safe zone file. If it does not exist, it will be created with a default safe zone.");
			Valheim_Online.ServerSaveInterval = base.Config.Bind<int>("VOnline", "ServerSaveInterval", 600, "SERVER ONLY: How often, in seconds, to save a copy of each character. Too low may result in performance issues. Too high may result in lost data in the event of a server crash.");
			if (!File.Exists(Valheim_Online.ServerSafeZonePath.Value))
			{
				Debug.Log(string.Format("Creating safe zone file at {0}", Valheim_Online.ServerSafeZonePath.Value));
				string text = "# format: name x z radius\nDefaultSpawnSafeZone 0.0 0.0 50.0";
				File.WriteAllText(Valheim_Online.ServerSafeZonePath.Value, text);
			}
			foreach (string text2 in File.ReadAllLines(Valheim_Online.ServerSafeZonePath.Value))
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

		public const string Name = "VOnline";

		public const string Guid = "Aluviel.VOnline";

		public const string Version = "0.1";

		public static ConfigEntry<string> ServerVaultPath;

		public static ConfigEntry<string> ServerSafeZonePath;

		public static ConfigEntry<int> ServerSaveInterval;
	}
}
