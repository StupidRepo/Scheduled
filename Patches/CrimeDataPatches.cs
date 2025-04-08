namespace Scheduled.Patches;

// [HarmonyPatch(typeof(PlayerCrimeData))]
// public class CrimeDataPatches
// {
// 	private static TimelineEventHandle? currentEventHandle;
// 	
// 	[HarmonyPatch(nameof(PlayerCrimeData.SetPursuitLevel))]
// 	[HarmonyPostfix]
// 	public static void SetPursuitLevel(PlayerCrimeData __instance, PlayerCrimeData.EPursuitLevel level)
// 	{
// 		if (!__instance.Player.IsLocalPlayer) return;
// 		Plugin.Logger.LogInfo($"Current pursuit level is {level}, we " + (currentEventHandle == null ? "DO NOT" : "") + " have an event handle.");
// 		
// 		if(level == PlayerCrimeData.EPursuitLevel.None) // if we are not in pursuit anymore.
// 		{
// 			// Update discord activity
// 			if (!SteamClient.IsValid || currentEventHandle == null) return; // if steam was not initialized or we don't have an event handle.
// 			
// 			Plugin.Logger.LogInfo("Ending timeline event!");
// 			SteamTimeline.EndRangeTimelineEvent(currentEventHandle.Value, 4);
// 			
// 			currentEventHandle = null;
// 		}
// 		else if(level != PlayerCrimeData.EPursuitLevel.Investigating) // If we are in pursuit
// 		{
// 			// Update discord activity
// 			if (!SteamClient.IsValid || currentEventHandle != null) return; // if steam was not initialized or we already have an event handle.
// 			
// 			Plugin.Logger.LogInfo("Starting timeline event!");
// 			currentEventHandle = SteamTimeline.StartRangeTimelineEvent(
// 				"Running from the police!",
// 				"👮",
// 				"steam_gem",
// 				10,
// 				-4,
// 				TimelineEventClipPriority.Standard
// 			);
// 		}
// 	}
// }