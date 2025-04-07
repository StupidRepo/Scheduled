using HarmonyLib;
using ScheduleOne.PlayerScripts;
using Steamworks;
using Steamworks.Data;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(PlayerCrimeData))]
public class CrimeDataPatches
{
	private static TimelineEventHandle? currentEventHandle;
	
	[HarmonyPatch(nameof(PlayerCrimeData.SetPursuitLevel))]
	[HarmonyPostfix]
	public static void SetPursuitLevel(PlayerCrimeData __instance, PlayerCrimeData.EPursuitLevel level)
	{
		if (!__instance.Player.IsLocalPlayer || !SteamClient.IsValid) return;
		Plugin.Logger.LogInfo($"Current pursuit level is {level}, we " + (currentEventHandle == null ? "DO NOT" : "") + " have an event handle.");

		if(level == PlayerCrimeData.EPursuitLevel.None && currentEventHandle != null) // If we are not in pursuit anymore.
		{
			Plugin.Logger.LogInfo("Ending timeline event!");
			SteamTimeline.EndRangeTimelineEvent(currentEventHandle.Value, 4);
			
			currentEventHandle = null;
		}
		else if((level != PlayerCrimeData.EPursuitLevel.None && level != PlayerCrimeData.EPursuitLevel.Investigating) && currentEventHandle == null) // If we are in pursuit and there is no current event.
		{
			Plugin.Logger.LogInfo("Starting timeline event!");
			currentEventHandle = SteamTimeline.StartRangeTimelineEvent(
				"Running from the police!",
				"👮",
				"steam_gem",
				10,
				-4,
				TimelineEventClipPriority.Standard
			);
		}
	}
}