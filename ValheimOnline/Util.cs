using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ValheimOnline
{

	public static class Util
	{

		public static void WriteCharacter(string path, byte[] data)
		{
			Debug.Log($"Writing character to {path}.");
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			using (FileStream fileStream = File.OpenWrite(path))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					binaryWriter.Write(data.Length);
					binaryWriter.Write(data);
				}
			}
		}

		public static string GetCharacterPathForSteamId(string id)
		{
			return Path.Combine(ValheimOnline.ServerVaultPath.Value, id, "current.voc");
		}

		// Compress (zip) the data
		public static ZPackage Compress(ZPackage package)
		{
			byte[] array = package.GetArray();
			MemoryStream memoryStream = new MemoryStream();
			GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
			gzipStream.Write(array, 0, array.Length);
			gzipStream.Close();
			memoryStream.Position = 0L;
			byte[] array2 = new byte[memoryStream.Length];
			memoryStream.Read(array2, 0, array2.Length);
			byte[] array3 = new byte[array2.Length + 4];
			Buffer.BlockCopy(array2, 0, array3, 4, array2.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(array.Length), 0, array3, 0, 4);
			return new ZPackage(array3);
		}

		// Decompress (zip) the data
		public static ZPackage Decompress(ZPackage package)
		{
			byte[] array = package.GetArray();
			MemoryStream memoryStream = new MemoryStream();
			int num = BitConverter.ToInt32(array, 0);
			memoryStream.Write(array, 4, array.Length - 4);
			byte[] array2 = new byte[num];
			memoryStream.Position = 0L;
			new GZipStream(memoryStream, CompressionMode.Decompress).Read(array2, 0, array2.Length);
			return new ZPackage(array2);
		}

		public static ZPackage Serialize(this PlayerProfile profile, Player player, bool logout_point = true)
		{
			if (logout_point)
			{
				profile.SetLogoutPoint(player.transform.position);
			}
			if (profile.m_playerID != 0L)
			{
				profile.SetMapData(Minimap.instance.GetMapData());
			}
			profile.SavePlayerData(player);
			ZPackage zpackage = new ZPackage();
			zpackage.Write(Version.m_playerVersion);
			zpackage.Write(profile.m_playerStats.m_kills);
			zpackage.Write(profile.m_playerStats.m_deaths);
			zpackage.Write(profile.m_playerStats.m_crafts);
			zpackage.Write(profile.m_playerStats.m_builds);
			zpackage.Write(profile.m_worldData.Count);
			foreach (KeyValuePair<long, PlayerProfile.WorldPlayerData> keyValuePair in profile.m_worldData)
			{
				zpackage.Write(keyValuePair.Key);
				zpackage.Write(keyValuePair.Value.m_haveCustomSpawnPoint);
				zpackage.Write(keyValuePair.Value.m_spawnPoint);
				zpackage.Write(keyValuePair.Value.m_haveLogoutPoint);
				zpackage.Write(keyValuePair.Value.m_logoutPoint);
				zpackage.Write(keyValuePair.Value.m_haveDeathPoint);
				zpackage.Write(keyValuePair.Value.m_deathPoint);
				zpackage.Write(keyValuePair.Value.m_homePoint);
				zpackage.Write(keyValuePair.Value.m_mapData != null);
				if (keyValuePair.Value.m_mapData != null)
				{
					zpackage.Write(keyValuePair.Value.m_mapData);
				}
			}
			zpackage.Write("");
			zpackage.Write(profile.m_playerID);
			zpackage.Write("");
			if (profile.m_playerData != null)
			{
				zpackage.Write(true);
				zpackage.Write(profile.m_playerData);
			}
			else
			{
				zpackage.Write(false);
			}
			return zpackage;
		}

		public static ZPackage Serialize(this List<ServerState.SafeZone> zones)
		{
			ZPackage zpackage = new ZPackage();
			zpackage.Write(zones.Count);
			foreach (ServerState.SafeZone safeZone in zones)
			{
				zpackage.Write(safeZone.name);
				zpackage.Write(safeZone.position.x);
				zpackage.Write(safeZone.position.y);
				zpackage.Write(safeZone.radius);
			}
			return zpackage;
		}

		public static ZPackage Serialize(this List<ServerState.BattleZone> zones)
		{
			ZPackage zpackage = new ZPackage();
			zpackage.Write(zones.Count);
			foreach (ServerState.BattleZone BattleZone in zones)
			{
				zpackage.Write(BattleZone.name);
				zpackage.Write(BattleZone.position.x);
				zpackage.Write(BattleZone.position.y);
				zpackage.Write(BattleZone.radius);
			}
			return zpackage;
		}

		public static void Deserialize(this PlayerProfile profile, ZPackage data)
		{
			Debug.Assert(data.ReadInt() <= Version.m_playerVersion);
			profile.m_playerStats.m_kills = data.ReadInt();
			profile.m_playerStats.m_deaths = data.ReadInt();
			profile.m_playerStats.m_crafts = data.ReadInt();
			profile.m_playerStats.m_builds = data.ReadInt();
			profile.m_worldData.Clear();
			int num = data.ReadInt();
			for (int i = 0; i < num; i++)
			{
				long key = data.ReadLong();
				PlayerProfile.WorldPlayerData worldPlayerData = (PlayerProfile.WorldPlayerData)Activator.CreateInstance(typeof(PlayerProfile.WorldPlayerData), true);
				worldPlayerData.m_haveCustomSpawnPoint = data.ReadBool();
				worldPlayerData.m_spawnPoint = data.ReadVector3();
				worldPlayerData.m_haveLogoutPoint = data.ReadBool();
				worldPlayerData.m_logoutPoint = data.ReadVector3();
				worldPlayerData.m_haveDeathPoint = data.ReadBool();
				worldPlayerData.m_deathPoint = data.ReadVector3();
				worldPlayerData.m_homePoint = data.ReadVector3();
				if (data.ReadBool())
				{
					worldPlayerData.m_mapData = data.ReadByteArray();
				}
				profile.m_worldData.Add(key, worldPlayerData);
			}
			profile.m_playerName = data.ReadString();
			profile.m_playerID = data.ReadLong();
			if (profile.m_playerID == 0L)
			{
				profile.m_playerID = Utils.GenerateUID();
			}
			profile.m_startSeed = data.ReadString();
			if (data.ReadBool())
			{
				profile.m_playerData = data.ReadByteArray();
			}
		}

		public static void Deserialize(this List<ServerState.SafeZone> zones, ZPackage package)
		{
			zones.Clear();
			int num = package.ReadInt();
			for (int i = 0; i < num; i++)
			{
				zones.Add(new ServerState.SafeZone
				{
					name = package.ReadString(),
					position = new Vector2(package.ReadSingle(), package.ReadSingle()),
					radius = package.ReadSingle()
				});
			}
		}

		public static void Deserialize(this List<ServerState.BattleZone> zones, ZPackage package)
		{
			zones.Clear();
			int num = package.ReadInt();
			for (int i = 0; i < num; i++)
			{
				zones.Add(new ServerState.BattleZone
				{
					name = package.ReadString(),
					position = new Vector2(package.ReadSingle(), package.ReadSingle()),
					radius = package.ReadSingle()
				});
			}
		}

		public static ZPackage LoadOrMakeCharacter(string steamid)
		{
			string characterPathForSteamId = Util.GetCharacterPathForSteamId(steamid);
			Debug.Log($"Attempting to load character {characterPathForSteamId}.");
			if (!File.Exists(characterPathForSteamId))
			{
				Debug.Log("Character does not exist, using default character.");
				Directory.CreateDirectory(Path.GetDirectoryName(characterPathForSteamId));
				File.WriteAllBytes(characterPathForSteamId, ServerState.default_character);
			}
            ZPackage result;
			using (FileStream fileStream = File.OpenRead(characterPathForSteamId))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					result = new ZPackage(binaryReader.ReadBytes(binaryReader.ReadInt32()));
				}
			}
			return result;
		}

		public static bool PointInSafeZone(Vector3 point, out ServerState.SafeZone zone)
		{
			Vector2 a = new Vector2(point.x, point.z);
			foreach (ServerState.SafeZone safeZone in ServerState.SafeZones)
			{
				if (Vector2.Distance(a, safeZone.position) <= safeZone.radius)
				{
					zone = safeZone;
					return true;
				}
			}
			zone = default(ServerState.SafeZone);
			return false;
		}

		public static bool PointInBattleZone(Vector3 point, out ServerState.BattleZone zone)
		{
			Vector2 a = new Vector2(point.x, point.z);
			foreach (ServerState.BattleZone battleZone in ServerState.BattleZones)
			{
				if (Vector2.Distance(a, battleZone.position) <= battleZone.radius)
				{
					zone = battleZone;
					return true;
				}
			}
			zone = default(ServerState.BattleZone);
			return false;
		}


		public static ServerState.ConnectionData GetServer()
		{
			Debug.Assert(!ZNet.instance.IsServer());
			Debug.Assert(ServerState.Connections.Count == 1);
			return ServerState.Connections[0];
		}

		private static MethodInfo func_Serialize = AccessTools.Method(typeof(PlayerProfile), "SavePlayerToDisk", null, null);

		private static MethodInfo func_Deserialize = AccessTools.Method(typeof(PlayerProfile), "LoadPlayerFromDisk", null, null);
	}
}
