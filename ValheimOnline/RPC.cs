
namespace ValheimOnline
{

	public static class RPC
	{

		public static void ServerVaultData(ZRpc rpc, ZPackage data)
		{
			Debug.Log("S2C ServerVaultData");
			Debug.Assert(!ZNet.instance.IsServer());
			Debug.Assert(ServerState.ClientPendingLoadData == null);
			ServerState.ClientPendingLoadData = data;
			ServerState.Connections.Add(new ServerState.ConnectionData
			{
				rpc = rpc
			});
		}

		public static void ServerVaultUpdate(ZRpc rpc, ZPackage data)
		{
			if (ZNet.instance.IsServer())
			{
				Debug.Log("C2S ServerVaultUpdate");
				string hostName = rpc.GetSocket().GetHostName();
				string characterPathForSteamId = Util.GetCharacterPathForSteamId(hostName);
				Debug.Log(string.Format("Saving character from SteamID {0}.", hostName));
				Util.WriteCharacter(characterPathForSteamId, Util.Decompress(data).GetArray());
				return;
			}
			Debug.Log("S2C ServerVaultUpdate");
			if (Player.m_localPlayer != null)
			{
				rpc.Invoke("ServerVaultUpdate", new object[]
				{
					Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
				});
				return;
			}
			Debug.Log("No local player yet: skipping.");
		}

		public static void ServerQuit(ZRpc rpc, ZPackage data)
		{
			if (ZNet.instance.IsServer())
			{
				Debug.Log("C2S ServerQuit");
				RPC.ServerVaultUpdate(rpc, data);
				rpc.Invoke("ServerQuit", new object[]
				{
					new ZPackage()
				});
				ServerState.Connections.RemoveAll((ServerState.ConnectionData conn) => conn.rpc.GetSocket() == rpc.GetSocket());
				Debug.Log("Connections " + ServerState.Connections.Count.ToString());
				return;
			}
			Debug.Log("S2C ServerQuit");
			ServerState.ClientMayDisconnect = true;
		}

		public static void SafeZones(ZRpc rpc, ZPackage data)
		{
			Debug.Log("S2C SafeZones");
			Debug.Assert(!ZNet.instance.IsServer());
			ServerState.SafeZones.Deserialize(data);
		}
    }
}
