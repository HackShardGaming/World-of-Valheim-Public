using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace VOnline
{

	[HarmonyPatch]
	public static class StandalonePatches
	{

		[HarmonyPrefix]
		[HarmonyPatch(typeof(ChangeLog), "Start")]
		private static void ChangeLog__Start(ref TextAsset ___m_changeLog)
		{
			string str = string.Format("2021-03-11 {0} v{1}\n", "VOnline", "0.1") + "* Server-side characters: when the client connects to the server, it is provided with a character to use by the server. The server maintains ownership of the client's character.\n* Safe Zones: In Valheim Online, most of the world is known as \"wilderness\". In the wilderness, PvP is forced on and map marker sharing off. Server admins can specify one or more safe zones which have PvP forced off.\n* Damn That Crow: The \"I have arrived\" shout does not belong in a persistent world. It has been removed.\n\n";
			___m_changeLog = new TextAsset(str + ___m_changeLog.text);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Version), "GetVersionString")]
		private static void Version__GetVersionString(ref string __result)
		{
			__result = string.Format("{0} ({1} v{2})", __result, "VOnline", "0.1");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(FejdStartup), "Update")]
		private static void FejdStartup__Update(GameObject ___m_startGamePanel, Button ___m_worldStart)
		{
			if (___m_startGamePanel.activeInHierarchy)
			{
				GameObject gameObject = GameObject.Find("Start");
				if (gameObject != null)
				{
					Text componentInChildren = gameObject.GetComponentInChildren<Text>();
					if (componentInChildren != null)
					{
						componentInChildren.text = "Disabled in Valheim Online";
					}
				}
				___m_worldStart.interactable = false;
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Game), "Update")]
		private static void Game__Update()
		{
			if (ZNet.instance.IsServer())
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				using (List<ServerState.ConnectionData>.Enumerator enumerator = ServerState.Connections.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ServerState.ConnectionData connectionData = enumerator.Current;
						if (realtimeSinceStartup - connectionData.last_save_time >= (float)Valheim_Online.ServerSaveInterval.Value)
						{
							connectionData.rpc.Invoke("ServerVaultUpdate", new object[]
							{
								new ZPackage()
							});
							connectionData.last_save_time = realtimeSinceStartup;
						}
					}
					return;
				}
			}
			if (Player.m_localPlayer)
			{
				ServerState.SafeZone safeZone;
				bool flag = Util.PointInSafeZone(Player.m_localPlayer.transform.position, out safeZone);
				if (flag && !ServerState.ClientInSafeZone)
				{
					Player.m_localPlayer.Message(MessageHud.MessageType.Center, string.Format("You have now entered safe zone {0}", safeZone.name), 0, null);
					ServerState.ClientInSafeZone = true;
				}
				else if (!flag && ServerState.ClientInSafeZone)
				{
					Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You are now in the wilderness", 0, null);
					ServerState.ClientInSafeZone = false;
				}
				Player.m_localPlayer.SetPVP(!flag);
				ZNet.instance.SetPublicReferencePosition(flag);
			}
			if (ServerState.ClientMayDisconnect)
			{
				Debug.Log(string.Format("Deferred log m_quitting: {0} m_logging: {1}", StandalonePatches.m_quitting, StandalonePatches.m_logging));
				if (StandalonePatches.m_quitting)
				{
					Menu.instance.OnQuitYes();
				}
				else
				{
					Menu.instance.OnLogoutYes();
				}
				ServerState.Connections.Clear();
				ServerState.ClientMayDisconnect = false;
				StandalonePatches.m_quitting = false;
				StandalonePatches.m_logging = false;
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(ZNet), "OnNewConnection")]
		private static void ZNet__OnNewConnection(ZNet __instance, ZNetPeer peer)
		{
			if (!__instance.IsServer())
			{
				peer.m_rpc.Register<ZPackage>("ServerVaultData", new Action<ZRpc, ZPackage>(RPC.ServerVaultData));
				peer.m_rpc.Register<ZPackage>("SafeZones", new Action<ZRpc, ZPackage>(RPC.SafeZones));
			}
			peer.m_rpc.Register<ZPackage>("ServerVaultUpdate", new Action<ZRpc, ZPackage>(RPC.ServerVaultUpdate));
			peer.m_rpc.Register<ZPackage>("ServerQuit", new Action<ZRpc, ZPackage>(RPC.ServerQuit));
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Player), "OnDeath")]
		private static void Player__OnDeath(Player __instance)
		{
			if (ZNet.instance.IsServer())
			{
				return;
			}
			if (__instance == Player.m_localPlayer)
			{
				Debug.Log("Char update (death)");
				PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
				playerProfile.ClearLoguoutPoint();
				Util.GetServer().rpc.Invoke("ServerVaultUpdate", new object[]
				{
					Util.Compress(playerProfile.Serialize(__instance, false))
				});
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(ZNet), "SendPeerInfo")]
		private static void ZNet__SendPeerInfo(ZNet __instance, ZRpc rpc)
		{
			if (!__instance.IsServer())
			{
				return;
			}
			Debug.Log("S2C ServerVaultData");
			rpc.Invoke("ServerVaultData", new object[]
			{
				Util.Compress(Util.LoadOrMakeCharacter(rpc.GetSocket().GetHostName()))
			});
			Debug.Log("S2C SafeZones");
			rpc.Invoke("SafeZones", new object[]
			{
				ServerState.SafeZones.Serialize()
			});
			ServerState.Connections.Add(new ServerState.ConnectionData
			{
				rpc = rpc
			});
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(Menu), "OnQuitYes")]
		private static bool Menu__OnQuitYes()
		{
			if (StandalonePatches.m_quitting)
			{
				return true;
			}
			StandalonePatches.m_quitting = true;
			Debug.Assert(!ZNet.instance.IsServer());
			Debug.Log("Quitting: sending ServerQuit and waiting.");
			Util.GetServer().rpc.Invoke("ServerQuit", new object[]
			{
				Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
			});
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(Menu), "OnLogoutYes")]
		private static bool Menu__OnLogoutYes()
		{
			if (StandalonePatches.m_logging)
			{
				return true;
			}
			StandalonePatches.m_logging = true;
			Debug.Assert(!ZNet.instance.IsServer());
			Debug.Log("Logging out: sending ServerQuit and waiting.");
			Util.GetServer().rpc.Invoke("ServerQuit", new object[]
			{
				Util.Compress(Game.instance.GetPlayerProfile().Serialize(Player.m_localPlayer, true))
			});
			return false;
		}

		private static bool m_quitting;

		private static bool m_logging;
	}
}
