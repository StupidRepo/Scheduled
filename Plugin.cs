using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Discord;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using Scheduled.Managers;
using Steamworks;

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
		SteamClient.Init(3164500);

		Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
	}
	private void Start()
	{
		StartCoroutine(OnSteamInit());
		DiscordManager.UpdateActivity(DEFAULT_ACTIVITY);
	}

	private void Update() => Discord.RunCallbacks();

	private void OnApplicationQuit() => SteamClient.Shutdown();

	private IEnumerator OnSteamInit()
	{
		while (!SteamClient.IsValid) { yield return null; }

		Logger.LogInfo("Steamworks is initialized, setting status to PLAYING!");
		SteamTimeline.SetTimelineGameMode(TimelineGameMode.Playing);
	}
}