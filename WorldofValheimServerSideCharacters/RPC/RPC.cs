
using System.Collections.Generic;
using ValheimPermissions;
using System;
using System.Text.RegularExpressions;

namespace WorldofValheimServerSideCharacters
{

	public static class RPC
	{

		public static void CharacterData(ZRpc rpc, ZPackage data)
		{
			Debug.Log("Server->Client CharacterData");
			Debug.Assert(!ZNet.instance.IsServer());
			Debug.Assert(ServerState.ClientLoadingData == null);
			ServerState.ClientLoadingData = data;
			if (!ZNet.instance.IsServer())
			{
				Debug.Log("Connection to server established! Changing ConnectionCount!");
				ServerState.ConnectionCount = 1;
				Debug.Log("Clearing all RPC connections");
				ServerState.Connections = new List<ServerState.ConnectionData>();
			}
			Debug.Log("Establishing RPC Connection");
			ServerState.Connections.Add(new ServerState.ConnectionData
			{
				rpc = rpc
			});
		}
		public static void SaveAll(long sender, ZPackage pkg)
		{
			ZNetPeer peer = ZNet.instance.GetPeer(sender);
			if (peer != null)
			{
				string permissionnode = "HackShardGaming.WoV-SSC.SaveAll";
				string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
				bool PlayerPermission = ValheimPermissions.ValheimDB.CheckUserPermission(peerSteamID, permissionnode);
				if (PlayerPermission)
				{
					Util.SaveAll();
				}
				else
				{
					Util.RoutedBroadcast(sender, $"Sorry! You do not have the permission to use !SaveAll (Required Permission: {permissionnode})");
				}
			}
		}
		public static void ShutdownServer(long sender, ZPackage pkg)
		{
			ZNetPeer peer = ZNet.instance.GetPeer(sender);
			if (peer != null)
			{
				string permissionnode = "HackShardGaming.WoV-SSC.ShutdownServer";
				string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
				bool PlayerPermission = ValheimPermissions.ValheimDB.CheckUserPermission(peerSteamID, permissionnode);
				if (PlayerPermission)
				{
					Game.instance.StartCoroutine(Util.ShutdownServer());
				}
				else
				{
					Util.RoutedBroadcast(sender, $"Sorry! You do not have the permission to use !ShutdownServer (Required Permission: {permissionnode})");
				}
			}
		}
		public static void ReloadDefault(long sender, ZPackage pkg)
		{
			ZNetPeer peer = ZNet.instance.GetPeer(sender);
			if (peer != null)
			{
				string permissionnode = "HackShardGaming.WoV-SSC.ReloadDefault";
				string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
				bool PlayerPermission = ValheimPermissions.ValheimDB.CheckUserPermission(peerSteamID, permissionnode);
				if (PlayerPermission)
				{
					Util.LoadOrMakeDefaultCharacter();
				}
				else
				{
					Util.RoutedBroadcast(sender, $"Sorry! You do not have the permission to use !ReloadDefault (Required Permission: {permissionnode})");
				}
			}
		}

		public static void CharacterUpdate(ZRpc rpc, ZPackage data)
		{
			// Are we the server? Update the character.
			if (ZNet.instance.IsServer())
			{
				Debug.Log("Client->Server CharacterUpdate");
				string hostName = rpc.GetSocket().GetHostName();
				ZNetPeer peer = ZNet.instance.GetPeerByHostName(hostName);
				string PlayerNameRaw = peer.m_playerName;
				string PlayerName = "";
				if (WorldofValheimServerSideCharacters.AllowMultipleCharacters.Value)
					PlayerName = Regex.Replace(PlayerNameRaw, @"<[^>]*>", String.Empty);
				else
					PlayerName = "Single_Character_Mode";
				string CharacterLocation = Util.GetCharacterPath(hostName, PlayerName);
				Debug.Log($"Saving character from SteamID {hostName}.");
				Util.WriteCharacter(CharacterLocation, Util.Decompress(data).GetArray());
				return;
			}
			// Are we the client? Send our character.
			Debug.Log("Server->Client CharacterUpdate");
			if (Player.m_localPlayer != null)
			{
				rpc.Invoke("CharacterUpdate", new object[]
				{
					Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
				});
				return;
			}
		}

		public static void ExitServer(ZRpc rpc, ZPackage data)
		{
			if (ZNet.instance.IsServer())
			{
				Debug.Log("Client->Server ExitServer");
				RPC.CharacterUpdate(rpc, data);
				rpc.Invoke("ExitServer", new object[]
				{
					new ZPackage()
				});
				Debug.Log($"Removing Client {rpc.GetSocket().GetHostName()} from our list");
				ServerState.Connections.RemoveAll((ServerState.ConnectionData conn) => conn.rpc.GetSocket() == rpc.GetSocket());
				Debug.Log("Connections " + ServerState.Connections.Count.ToString());
				return;
			}
			Debug.Log("Server->Client ExitServer");
            ServerState.Connections.RemoveAll((ServerState.ConnectionData conn) => conn.rpc.GetSocket() == rpc.GetSocket());
			ServerState.ClientCanDC = true;
		}
	}
}
