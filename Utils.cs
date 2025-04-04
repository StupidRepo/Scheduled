using Steamworks;

namespace Scheduled;

public static class Utils
{
	public static void AddNewEvent(string name, string description, string icon,
		float offset, float duration,
		uint priority = 1,
		ETimelineEventClipPriority type = ETimelineEventClipPriority.k_ETimelineEventClipPriority_Standard
	) {
		if(!SteamManager.Initialized) return;
		
		Plugin.Logger.LogInfo("Adding new event!");
		SteamTimeline.AddTimelineEvent(
			icon,
			name,
			description,
			priority,
			offset,
			duration,
			type
		);
	}
}