using HarmonyLib;
using ScheduleOne.PlayerScripts;
using Steamworks;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(Player))]
public class PlayerPatches
{
	[HarmonyPatch(nameof(Player.OnDied))]
	[HarmonyPostfix]
	public static void OnDiedPostfix(Player __instance)
	{
		if (!__instance.Owner.IsLocalClient) return;
		SteamTimeline.AddTimelineEvent(
			"steam_death",
			"Died",
			"You died!",
			2,
			0,
			0,
			ETimelineEventClipPriority.k_ETimelineEventClipPriority_Featured
		);
	}
}