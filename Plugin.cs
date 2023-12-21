using BepInEx;
using HarmonyLib;
using System.Reflection;
using LC_API;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Animations;

namespace GiantChibi
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("LC_API")]
    [BepInProcess("Lethal Company.exe")]
    public class Plugin : BaseUnityPlugin
	{
		public static Harmony _harmony;

		private void Awake()
		{
			_harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
			_harmony.PatchAll();
			Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} loaded");
		}

		[HarmonyPatch(typeof(ForestGiantAI))]
		internal class ForestGiantAIPatch
		{
			[HarmonyPatch("Start")]
			[HarmonyPostfix]
			public static void StartPatch(ref PlayerControllerB __instance)
			{
				__instance.gameObject.AddComponent<ChibiModel>();
			}
		}
	}
}