using BepInEx.Logging;
using Discord;
using LogLevel = Discord.LogLevel;

namespace Scheduled.Managers;

public class DiscordManager
{
	public readonly Discord.Discord Discord;
	private readonly ManualLogSource logger = new("Discord Game SDK");
	
	public DiscordManager()
	{
		Discord = new Discord.Discord(1358163596479303941, (ulong)CreateFlags.NoRequireDiscord);
		Discord.SetLogHook(LogLevel.Info, (level, message) =>
		{
			if (level == LogLevel.Warn) logger.LogWarning(message);
			else if (level == LogLevel.Error) logger.LogError(message);
			else logger.LogInfo(message);
		});
		
		var activityManager = Discord.GetActivityManager();
		var activity = new Activity
		{
			State = "🌿🚬",
			Timestamps =
			{
				Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			}
		};
		
		activityManager.UpdateActivity(activity, result =>
		{
			Console.WriteLine(result == Result.Ok
				? "Activity updated successfully."
				: $"Failed to update activity: {result}");
		});
	}
}