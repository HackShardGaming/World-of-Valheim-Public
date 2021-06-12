﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;

namespace ServerSideCharacters
{
    public static class Util
    {
        public static bool isServer()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }

        public static void LoadOrMakeDefaultCharacter()
        {
            if (!File.Exists(ServerSideCharacters.DefaultCharacterPath.Value))
            {
                Debug.Log($"Default character file doesn't exist, creating file at {ServerSideCharacters.DefaultCharacterPath.Value}");
                Directory.CreateDirectory(Path.GetDirectoryName(ServerSideCharacters.DefaultCharacterPath.Value));
                File.WriteAllBytes(ServerSideCharacters.DefaultCharacterPath.Value,
                    global::ServerSideCharacters.Properties.Resources._default_character);
            }
            else
            {
                Debug.Log($"Loading default character file from {ServerSideCharacters.DefaultCharacterPath.Value}");
                ServerState.default_character = File.ReadAllBytes(ServerSideCharacters.DefaultCharacterPath.Value);
            }

            Debug.Log($"Loaded default character file (Size: {ServerState.default_character.Length})");
        }

        public static void Broadcast(string text, string username = ModInfo.Title)
        {
            Debug.Log($"Broadcasting {text}");
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }

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

        public static string GetCharacterPath(string id, string playerName)
        {
            // This is where we would put an update to change character files depending on what character name they are using client side.
            return Path.Combine(ServerSideCharacters.CharacterSavePath.Value, id, playerName + ".wov");
        }

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
            byte[] result = new byte[array2.Length + 4];
            Buffer.BlockCopy(array2, 0, result, 4, array2.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(array.Length), 0, result, 0, 4);
            return new ZPackage(result);
        }

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
                PlayerProfile.WorldPlayerData worldPlayerData = (PlayerProfile.WorldPlayerData)Activator
                    .CreateInstance(typeof(PlayerProfile.WorldPlayerData), true);
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

        public static ZPackage LoadOrMakeCharacter(string steamid)
        {
            ZNetPeer peer = ZNet.instance.GetPeerByHostName(steamid);
            string PlayerNameRaw = peer.m_playerName;
            string PlayerName = String.Empty;
            if (ServerSideCharacters.AllowMultipleCharacters.Value)
                PlayerName = Regex.Replace(PlayerNameRaw, @"<[^>]*>", String.Empty);
            else
                PlayerName = "Single_Character_Mode";
            string CharacterLocation = Util.GetCharacterPath(steamid, PlayerName);
            Debug.Log($"Attempting to load the character for SteamID.CharacterName: {steamid}.{PlayerName}");
            if (!File.Exists(CharacterLocation))
            {
                Debug.Log("That character does not exist! Loading them up a fresh default!");
                Directory.CreateDirectory(Path.GetDirectoryName(CharacterLocation));
                File.WriteAllBytes(CharacterLocation, ServerState.default_character);
            }
            ZPackage result;
            using (FileStream fileStream = File.OpenRead(CharacterLocation))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    result = new ZPackage(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                }
            }
            return result;
        }

        public static ServerState.ConnectionData GetServer()
        {
            Debug.Assert(!ZNet.instance.IsServer());
            Debug.Assert(ServerState.Connections.Count == 1);
            return ServerState.Connections[0];
        }

        public static void SaveAll()
        {
            using (List<ServerState.ConnectionData>.Enumerator enumerator = ServerState.Connections.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ServerState.ConnectionData connectionData = enumerator.Current;
                    connectionData.rpc.Invoke("CharacterUpdate", new object[]
                    {
                        new ZPackage()
                    });
                }
            }
        }

        public static void DisconnectAll()
        {
            using (List<ServerState.ConnectionData>.Enumerator enumerator = ServerState.Connections.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ServerState.ConnectionData connectionData = enumerator.Current;
                    connectionData.rpc.Invoke("Disconnect", Array.Empty<object>());
                }
            }
        }
        public static ZNet.PlayerInfo getBasicPlayerInfoFromPlayer(Player player) => new ZNet.PlayerInfo
        {
            m_characterID = player.GetZDOID(),
            m_name = player.GetPlayerName(),
            m_position = player.transform.position
        };
        public static IEnumerator ShutdownServer()
        {
            if (ServerSideCharacters.RunningOnServer)
            {
                Broadcast("Server is being shutdown! Saving all characters now!");
                SaveAll();
                StandalonePatches.m_quitting = true;
                ZNet.instance.Save(true);
                DisconnectAll();
                yield return new WaitForSeconds(10);
                Application.Quit();
                System.Console.Out.Close();
            }
        }

        private static MethodInfo func_Serialize = AccessTools.Method(typeof(PlayerProfile), "SavePlayerToDisk", null, null);

        private static MethodInfo func_Deserialize = AccessTools.Method(typeof(PlayerProfile), "LoadPlayerFromDisk", null, null);
    }
}
