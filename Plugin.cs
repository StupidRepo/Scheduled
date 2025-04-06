using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Scheduled.Commands;
using Steamworks;
using Console = ScheduleOne.Console;

namespace Scheduled;

[BepInPlugin(PLUGIN_GUID, "Scheduled", VERSION)]
public class Plugin : BaseUnityPlugin
{
	// Plugin Info
	internal const string PLUGIN_GUID = "io.github.stupidrepo.Scheduled";
	internal const string VERSION = "1.0.0";
	
	// Shared Logger
	internal new static ManualLogSource Logger;
	
	// InputActions
	// private InputAction testAction;
	
	private void Awake()
	{
		// testAction = new InputAction("MyAction", InputActionType.Button, "<Keyboard>/space");
		Logger = base.Logger;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		
		Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
	}
	private void Start()
	{
		StartCoroutine(OnSteamInit());
	}

	private IEnumerator OnSteamInit()
	{
		while (!SteamManager.Initialized) { yield return null; }

		Logger.LogInfo("Steamworks is initialized, setting status to PLAYING!");
		SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Playing);
	}
}