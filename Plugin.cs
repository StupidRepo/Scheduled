using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Discord.Sdk;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace Scheduled;

[BepInPlugin(PLUGIN_GUID, "Scheduled", VERSION)]
public class Plugin : BaseUnityPlugin
{
	// Plugin Info
	internal const string PLUGIN_GUID = "io.github.stupidrepo.Scheduled";
	internal const string VERSION = "1.0.0";
	
	// Shared Logger
	internal new static ManualLogSource Logger;
	
	// Discord SDK
	internal static DiscordManager DiscordManager;
	
	// InputActions
	// private InputAction testAction;
	
	private void Awake()
	{
		// testAction = new InputAction("MyAction", InputActionType.Button, "<Keyboard>/space");
		Logger = base.Logger;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		
		// Init discord SDK
		DiscordManager = new DiscordManager();
		DiscordManager.OnReady += client =>
		{
			var activity = new Activity();
			activity.SetType(ActivityTypes.Playing);
			activity.SetState("🌿🚬");
			
			client.UpdateRichPresence(activity, result => {
				if (!result.Successful())
					Logger.LogError("Failed to update Discord Rich Presence: " + result.Error());
			});
		};

		Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
	}
	private void Start()
	{
		StartCoroutine(OnSteamInit());
		DiscordManager.StartOAuthFlow();
	}

	private IEnumerator OnSteamInit()
	{
		while (!SteamManager.Initialized) { yield return null; }

		Logger.LogInfo("Steamworks is initialized, setting status to PLAYING!");
		SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Playing);
	}
}