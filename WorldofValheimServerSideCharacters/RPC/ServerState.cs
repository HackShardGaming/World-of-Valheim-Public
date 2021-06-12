using System.Collections.Generic;
using UnityEngine;

namespace ServerSideCharacters
{

	public static class ServerState
	{
		public static int ConnectionCount = 0;
		public static ZPackage ClientLoadingData = null;

		public static bool ClientCanDC = false;


        public static byte[] default_character = global::ServerSideCharacters.Properties.Resources._default_character;

		public static List<ServerState.ConnectionData> Connections = new List<ServerState.ConnectionData>();

		public class ConnectionData
		{

			public ZRpc rpc;

			public float LastTimeSaved;
		}
	}
}
