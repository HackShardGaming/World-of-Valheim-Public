
using System.Collections.Generic;

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
				string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
				if (
					ZNet.instance.m_adminList != null &&
					ZNet.instance.m_adminList.Contains(peerSteamID)
				)
				{
					Util.SaveAll();
				}
			}
		}
		public static void ShutdownServer(long sender, ZPackage pkg)
		{
			ZNetPeer peer = ZNet.instance.GetPeer(sender);
			if (peer != null)
			{
				string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
				if (
					ZNet.instance.m_adminList != null &&
					ZNet.instance.m_adminList.Contains(peerSteamID)
				)
				{
					Debug.Log($"Shutting down the server");
					Game.instance.StartCoroutine(Util.ShutdownServer());
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
				string PlayerName = peer.m_playerName;
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
				Debug.Log($"Removing Client {rpc.GetSocket()} from our list");
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
