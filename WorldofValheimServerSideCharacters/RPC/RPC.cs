
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace ServerSideCharacters
{

	public static class RPC
	{

		public static void CharacterData(ZRpc rpc, ZPackage data)
		{
			Debug.Log("Sending CharacterData from server to client");
			Debug.Assert(!ZNet.instance.IsServer());
			Debug.Assert(ServerState.ClientLoadingData == null);
			ServerState.ClientLoadingData = data;
			if (!ZNet.instance.IsServer())
			{
				Debug.Log("Connection to server established!");
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

		public static void CharacterUpdate(ZRpc rpc, ZPackage data)
		{
			// Are we the server? Update the character.
			if (ZNet.instance.IsServer())
			{
				Debug.Log("Received CharacterUpdate from client");
				string steamId = rpc.GetSocket().GetHostName();
				ZNetPeer client = ZNet.instance.GetPeerByHostName(steamId);
				string PlayerName = "Single_Character_Mode";
				if (ServerSideCharacters.AllowMultipleCharacters.Value)
					PlayerName = Regex.Replace(client.m_playerName, @"<[^>]*>", String.Empty);
				string CharacterLocation = Util.GetCharacterPath(steamId, PlayerName);
				Debug.Log($"Saving character from SteamID {steamId}.");
				Util.WriteCharacter(CharacterLocation, Util.Decompress(data).GetArray());
			}

			// Are we the client? Send our character.
			if (Player.m_localPlayer != null)
			{
				Debug.Log("Sending CharacterUpdate to server");
				rpc.Invoke("CharacterUpdate", new object[]
				{
					Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
				});
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
