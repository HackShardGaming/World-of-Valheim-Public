﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ValheimOnline
{

	public static class ServerState
	{

		public static ZPackage ClientPendingLoadData = null;

		public static bool ClientMayDisconnect = false;

		public static bool ClientInSafeZone = false;

		public static List<ServerState.ConnectionData> Connections = new List<ServerState.ConnectionData>();

		public static List<ServerState.SafeZone> SafeZones = new List<ServerState.SafeZone>();

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
	}
}