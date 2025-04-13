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
using ScheduleOne.DevUtilities;
using ScheduleOne.Persistence;
using ScheduleOne.PlayerScripts;
using ScheduleOne.Product.Packaging;
using ScheduleOne.UI.MainMenu;
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
	internal static GameServerManager? GSManager;
	internal static AppId_t AppId = new(3164500);
	
	// Network Prefabs
	private PrefabObjects NetworkedPrefabs;
	
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
		StartCoroutine(OnSteamInit());
		
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
		DiscordManager?.UpdateActivity(DEFAULT_ACTIVITY);
		DiscordManager?.ActivityManager.RegisterSteam(AppId.m_AppId);
		
		NetworkedPrefabs = InstanceFinder.NetworkManager.GetPrefabObjects<SinglePrefabObjects>(1337, createIfMissing: true);
		if (NetworkedPrefabs != null)
		{
			RegisterNetworkObject("VoiceChatManager");
		}
		else Logger.LogError("Networked Prefabs not found!");

		if (!Config.DedicatedServerMode.Value) return;
		if (string.IsNullOrEmpty(Config.ServerLoginToken.Value))
		{
			if(!MainMenuPopup.InstanceExists) return;
			MainMenuPopup.Instance.Open("Error", "You need to supply a Steam Game Server Login Token. Check the config for a link to the URL!", true);
		}
		GSManager = new GameServerManager();
	}

	private void Update()
	{
		Discord?.RunCallbacks();
		
		if ((GSManager?.IsInit).GetValueOrDefault(false))
		{
			GameServer.RunCallbacks();
		}
	}

	private void RegisterNetworkObject(string assetName)
	{
		if (NetworkedPrefabs == null)
		{
			Logger.LogError("NetworkedPrefabs is null!");
			return;
		}
		
		var go = AssetManager.GetAsset<GameObject>(assetName);
		if (go == null)
		{
			Logger.LogError($"Prefab {assetName} not found!");
			return;
		}
		
		var no = go.GetComponent<NetworkObject>();
		if (no == null)
		{
			Logger.LogError($"Prefab {assetName} does not have a NetworkObject component!");
			return;
		}
		NetworkedPrefabs.AddObject(no, checkForDuplicates: true);
	}

	private void OnApplicationQuit() => SteamAPI.Shutdown();
	
	private IEnumerator OnSteamInit()
	{
		while (!SteamworksManager.IsInit) { yield return null; }
		
		Logger.LogInfo("Steamworks is initialized!");
		SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Menus);
		if (LoadManager.InstanceExists)
		{
			LoadManager.Instance.onLoadComplete.AddListener(() =>
			{
				Logger.LogInfo("Setting game mode to Playing");
				SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Playing);
				
				var vcm = AssetManager.GetAsset<GameObject>("VoiceChatManager");
				if (vcm == null || !InstanceFinder.IsServer) return;
				
				var vcmGo = Instantiate(vcm);
				InstanceFinder.ServerManager.Spawn(vcmGo);
				
				Logger.LogInfo("VoiceChatManager spawned!");
			});
		}
	}
}