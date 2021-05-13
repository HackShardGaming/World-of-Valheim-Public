using HarmonyLib;
using UnityEngine;

namespace WorldofValheimServerSideCharacters
{
	[HarmonyPatch]
	public static class LoadCharacter
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Game), "UpdateRespawn")]
		private static void Game__UpdateRespawn(Game __instance)
		{
			if (__instance.m_requestRespawn && ServerState.ClientLoadingData != null)
			{
				Debug.Log("Attempting to load character...");
				ZNetView.m_forceDisableInit = true;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.m_playerPrefab);
				ZNetView.m_forceDisableInit = false;
				Player component = gameObject.GetComponent<Player>();
				__instance.GetPlayerProfile().LoadPlayerData(component);
				LoadCharacter.m_original_customization = new LoadCharacter.OriginalCustomization
				{
					name = __instance.GetPlayerProfile().GetName(),
					beard = component.GetBeard(),
					hair = component.GetHair(),
					skin_colour = component.m_skinColor,
					hair_colour = component.m_hairColor,
					model = component.GetPlayerModel()
				};
				UnityEngine.Object.DestroyImmediate(gameObject);
				__instance.GetPlayerProfile().Deserialize(Util.Decompress(ServerState.ClientLoadingData));
				__instance.GetPlayerProfile().SetName(LoadCharacter.m_original_customization.name);
				Minimap.instance.LoadMapData();
				ServerState.ClientLoadingData = null;
				Debug.Log($"Load Successful! Welcome {__instance.GetPlayerProfile().GetName()}!");
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Game), "SpawnPlayer")]
		private static void Game__SpawnPlayer(Player __result)
		{
			if (LoadCharacter.m_original_customization != null)
			{
				Debug.Log("Setting player settings...");
				__result.SetBeard(LoadCharacter.m_original_customization.beard);
				__result.SetHair(LoadCharacter.m_original_customization.hair);
				__result.SetSkinColor(LoadCharacter.m_original_customization.skin_colour);
				__result.SetHairColor(LoadCharacter.m_original_customization.hair_colour);
				__result.SetPlayerModel(LoadCharacter.m_original_customization.model);
				LoadCharacter.m_original_customization = null;
			}
		}

		private static LoadCharacter.OriginalCustomization m_original_customization;

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
