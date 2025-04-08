using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Discord;
using HarmonyLib;
using Scheduled.Managers;
using ScheduleOne.Persistence;
using Steamworks;
using UnityEngine.SceneManagement;

namespace Scheduled;

[BepInPlugin(PLUGIN_GUID, "Scheduled", VERSION)]
public class Plugin : BaseUnityPlugin
{
	// Plugin Info
	internal const string PLUGIN_GUID = "io.github.stupidrepo.Scheduled";
	internal const string VERSION = "1.0.0";
	
	// Shared Logger
	internal new static ManualLogSource Logger;
	
	// Discord Game SDK
	internal static DiscordManager DiscordManager;
	internal static Discord.Discord Discord => DiscordManager.Discord;
	
	// Steamworks
	internal static SteamworksManager SteamworksManager;
	
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
		Logger = base.Logger;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		
		// init external stuff
		DiscordManager = new DiscordManager();
		SteamworksManager = new SteamworksManager();
		
		// scene change stuff
		SceneManager.activeSceneChanged += (_, to) =>
		{
			if (to.name != "Menu" || !SteamManager.Initialized) return;
			
			Logger.LogDebug("Scene changed to Menu");
			SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Menus);
		};

		Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
	}
	private void Start()
	{
		StartCoroutine(OnSteamInit());
		DiscordManager.UpdateActivity(DEFAULT_ACTIVITY);
	}

	private void Update()
	{
		Discord.RunCallbacks();
	}

	private void OnApplicationQuit() => SteamAPI.Shutdown();
	
	private IEnumerator OnSteamInit()
	{
		while (!SteamManager.Initialized) { yield return null; }
		
		Logger.LogInfo("Steamworks is initialized!");
		
		SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Menus);
		if (LoadManager.Instance is not null)
		{
			LoadManager.Instance.onLoadComplete.AddListener(() =>
			{
				Logger.LogInfo("Setting game mode to Playing");
				SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Playing);
			});
		}
	}
}