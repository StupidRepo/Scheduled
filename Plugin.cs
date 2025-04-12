using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using Discord;
using FishNet;
using FishNet.Managing.Object;
using FishNet.Object;
using HarmonyLib;
using Scheduled.Managers;
using Scheduled.Test;
using ScheduleOne;
using ScheduleOne.Persistence;
using ScheduleOne.Product.Packaging;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scheduled;
#pragma warning disable BepInEx002

[BepInPlugin(PLUGIN_GUID, "Scheduled", VERSION)]
public class Plugin : BaseUnityPlugin
{
	// Plugin Info
	internal const string PLUGIN_GUID = "io.github.stupidrepo.Scheduled";
	internal const string VERSION = "1.0.0";
	
	// Shared Stuff
	internal new static ManualLogSource Logger;
	internal new static ScheduledConfig Config;
	internal static AssetManager AssetManager;
	
	// Discord Game SDK
	internal static DiscordManager? DiscordManager;
	internal static Discord.Discord? Discord => DiscordManager?.Discord;
	
	// Steamworks
	internal static SteamworksManager SteamworksManager;
	
	// Network Prefabs
	// private PrefabObjects NetworkedPrefabs;
	
	internal static Activity DEFAULT_ACTIVITY = new()
	{
		Type = ActivityType.Playing,
		
		State = DiscordManager.STATE,
		Details = "🌿🚬",
		
		Timestamps =
		{
			Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
		}
	};
	
	private void Awake()
	{
		// set Logger and do patching
		Config = new ScheduledConfig(base.Config);
		Logger = base.Logger;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		
		// init external stuff
		if(Config.InteractWithDiscord.Value)
			DiscordManager = new DiscordManager();
		SteamworksManager = new SteamworksManager();
		
		// scene change stuff
		SceneManager.activeSceneChanged += (_, to) =>
		{
			if (to.name != "Menu" || !SteamManager.Initialized) return;
			
			Logger.LogDebug("Scene changed to Menu");
			SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Menus);
		};

		AssetManager = new AssetManager("Scheduled.Assets.sigmabundle");
		
		Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
	}
	private void Start()
	{
		StartCoroutine(OnSteamInit());
		
		DiscordManager?.UpdateActivity(DEFAULT_ACTIVITY);
		DiscordManager?.ActivityManager.RegisterSteam(3164500);
		
		// NetworkedPrefabs = InstanceFinder.NetworkManager.GetPrefabObjects<SinglePrefabObjects>(1337, createIfMissing: true);
		// if (NetworkedPrefabs != null)
		// {
		// 	RegisterNetworkObject("VoiceChatManager");
		// }
		// else Logger.LogError("Networked Prefabs not found!");
		//
		// var mainMenuRig = FindObjectOfType<MainMenuRig>();
		// if (mainMenuRig == null) return;
		//
		// var cubey = AssetManager.GetAsset<GameObject>("Prefab_AnimBox");
		// if (cubey != null) Instantiate(cubey, mainMenuRig.Avatar.MiddleSpine);
		// else Logger.LogError("Cubey prefab not found!");

		// if (!Registry.InstanceExists) return;
		// var registry = Registry.Instance;
		// var boxPackagingDef = AssetManager.GetAsset<PackagingDefinition>("CoolJar");
		// if (boxPackagingDef == null)
		// {
		// 	Logger.LogError("Box packaging definition not found!");
		// 	return;
		// }
		//
		// registry.AddToRegistry(boxPackagingDef);
	}

	private void Update()
	{
		Discord?.RunCallbacks();
	}

	// private void RegisterNetworkObject(string assetName)
	// {
	// 	if (NetworkedPrefabs == null)
	// 	{
	// 		Logger.LogError("NetworkedPrefabs is null!");
	// 		return;
	// 	}
	// 	
	// 	var go = AssetManager.GetAsset<GameObject>(assetName);
	// 	if (go == null)
	// 	{
	// 		Logger.LogError($"Prefab {assetName} not found!");
	// 		return;
	// 	}
	// 	
	// 	var no = go.GetComponent<NetworkObject>();
	// 	if (no == null)
	// 	{
	// 		Logger.LogError($"Prefab {assetName} does not have a NetworkObject component!");
	// 		return;
	// 	}
	// 	NetworkedPrefabs.AddObject(no, checkForDuplicates: true);
	// }

	private void OnApplicationQuit() => SteamAPI.Shutdown();
	
	private IEnumerator OnSteamInit()
	{
		while (!SteamManager.Initialized) { yield return null; }
		
		Logger.LogInfo("Steamworks is initialized!");
		SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Menus);
		if (LoadManager.InstanceExists)
		{
			LoadManager.Instance.onLoadComplete.AddListener(() =>
			{
				Logger.LogInfo("Setting game mode to Playing");
				SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Playing);
				
				var vcm = AssetManager.GetAsset<GameObject>("VoiceChatManager");
				if (vcm != null && InstanceFinder.IsServer)
				{
					var vcmGo = Instantiate(vcm);
					InstanceFinder.ServerManager.Spawn(vcmGo);
					Logger.LogInfo("VoiceChatManager spawned!");
				}
			});
		}
	}
}