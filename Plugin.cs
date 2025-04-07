using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
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
	
	private void Awake()
	{
		// testAction = new InputAction("MyAction", InputActionType.Button, "<Keyboard>/space");
		DiscordManager = new DiscordManager();
		
		Logger = base.Logger;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		
		SteamClient.Init(3164500);
		Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
	}
	private void Start()
	{
		StartCoroutine(OnSteamInit());
	}

	private void OnApplicationQuit()
	{
		SteamClient.Shutdown();
	}

	private IEnumerator OnSteamInit()
	{
		while (!SteamClient.IsValid) { yield return null; }

		Logger.LogInfo("Steamworks is initialized, setting status to PLAYING!");
		SteamTimeline.SetTimelineGameMode(TimelineGameMode.Playing);
	}
}