using System.Collections.Generic;
using UnityEngine;

namespace ValheimOnline
{

	public static class ServerState
	{

		public static ZPackage ClientPendingLoadData = null;

		public static bool ClientMayDisconnect = false;

		//public static bool ClientInSafeZone = false;
		//public static bool ClientInBattleZone = false;

		//public static bool PVPEnforced = true;
        //public static bool PVPMode = false;
        //public static bool PVPSharePosition = true;
		//public static bool ServerForcePVP = false;

        public static byte[] default_character = global::ValheimOnline.Properties.Resources._default_character;

		public static List<ServerState.ConnectionData> Connections = new List<ServerState.ConnectionData>();

		public static List<ServerState.SafeZone> SafeZones = new List<ServerState.SafeZone>();

		public static List<ServerState.BattleZone> BattleZones = new List<ServerState.BattleZone>();

		public class ConnectionData
		{

			public ZRpc rpc;

			public float last_save_time;
		}

		public struct SafeZone
		{

			public string name;

			public Vector2 position;

			public float radius;
		}
        public struct BattleZone
        {

            public string name;

            public Vector2 position;

            public float radius;
        }
	}
}
