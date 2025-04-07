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
		SteamTimeline.AddInstantaneousTimelineEvent(
			"Died",
			"You died!",
			"steam_death",
			2,
			0,
			TimelineEventClipPriority.Featured
		);
	}
}