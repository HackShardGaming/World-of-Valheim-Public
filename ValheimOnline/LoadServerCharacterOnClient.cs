using HarmonyLib;
using UnityEngine;

namespace ValheimOnline
{
	[HarmonyPatch]
	public static class LoadServerCharacterOnClient
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Game), "UpdateRespawn")]
		private static void Game__UpdateRespawn(Game __instance)
		{
			if (__instance.m_requestRespawn && ServerState.ClientPendingLoadData != null)
			{
				Debug.Log("Loading pending character data.");
				ZNetView.m_forceDisableInit = true;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.m_playerPrefab);
				ZNetView.m_forceDisableInit = false;
				Player component = gameObject.GetComponent<Player>();
				__instance.GetPlayerProfile().LoadPlayerData(component);
				LoadServerCharacterOnClient.m_original_customization = new LoadServerCharacterOnClient.OriginalCustomization
				{
					name = __instance.GetPlayerProfile().GetName(),
					beard = component.GetBeard(),
					hair = component.GetHair(),
					skin_colour = component.m_skinColor,
					hair_colour = component.m_hairColor,
					model = component.GetPlayerModel()
				};
				UnityEngine.Object.DestroyImmediate(gameObject);
				__instance.GetPlayerProfile().Deserialize(Util.Decompress(ServerState.ClientPendingLoadData));
				__instance.GetPlayerProfile().SetName(LoadServerCharacterOnClient.m_original_customization.name);
				Minimap.instance.LoadMapData();
				ServerState.ClientPendingLoadData = null;
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Game), "SpawnPlayer")]
		private static void Game__SpawnPlayer(Player __result)
		{
			if (LoadServerCharacterOnClient.m_original_customization != null)
			{
				Debug.Log("Applying local character customizations");
				__result.SetBeard(LoadServerCharacterOnClient.m_original_customization.beard);
				__result.SetHair(LoadServerCharacterOnClient.m_original_customization.hair);
				__result.SetSkinColor(LoadServerCharacterOnClient.m_original_customization.skin_colour);
				__result.SetHairColor(LoadServerCharacterOnClient.m_original_customization.hair_colour);
				__result.SetPlayerModel(LoadServerCharacterOnClient.m_original_customization.model);
				LoadServerCharacterOnClient.m_original_customization = null;
			}
		}

		private static LoadServerCharacterOnClient.OriginalCustomization m_original_customization;

		private class OriginalCustomization
		{
			public string name;

			public string beard;

			public string hair;

			public Vector3 skin_colour;

			public Vector3 hair_colour;

			public int model;
		}
	}
}
