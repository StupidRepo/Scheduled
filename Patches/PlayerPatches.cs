using HarmonyLib;
using ScheduleOne.PlayerScripts;
using Steamworks;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(Player))]
public class PlayerPatches
{
	public const int TIME_BEFORE_EVENT = 5;
	public const int TIME_AFTER_EVENT = 2;
	
	[HarmonyPatch(nameof(Player.OnDied))]
	[HarmonyPostfix]
	public static void OnDiedPostfix(Player __instance)
	{
		if (!__instance.Owner.IsLocalClient) return;
		Utils.AddNewEvent(
			"Died",
			"You died!",
			"steam_death",
			-TIME_BEFORE_EVENT,
			TIME_BEFORE_EVENT+TIME_AFTER_EVENT, // records for 5sec before event, then 2sec after
			5,
			ETimelineEventClipPriority.k_ETimelineEventClipPriority_Featured
		);
	}
}