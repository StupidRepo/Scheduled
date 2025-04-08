using BepInEx.Logging;
using Discord;
using Steamworks;
using Lobby = Steamworks.Data.Lobby;
using LogLevel = Discord.LogLevel;
using Result = Discord.Result;

namespace Scheduled.Managers;

public class DiscordManager
{
	public const string STATE = "Playing Schedule I";
	
	public readonly Discord.Discord Discord;
	public readonly ActivityManager ActivityManager;
	private readonly ManualLogSource logger = new("Discord Game SDK");
	
	public DiscordManager()
	{
		Discord = new Discord.Discord(1358163596479303941, (ulong)CreateFlags.NoRequireDiscord);
		Discord.SetLogHook(LogLevel.Info, (level, message) =>
		{
			if (level == LogLevel.Warn) logger.LogWarning(message);
			else if (level == LogLevel.Error) logger.LogError(message);
			else logger.LogDebug(message);
		});
		
		ActivityManager = Discord.GetActivityManager();
	}

	public void UpdateActivity(Activity activity)
	{
		ActivityManager.UpdateActivity(activity, result =>
		{
			if (result != Result.Ok)
			{
				logger.LogError("Failed to update activity: " + result);
			}
		});
	}
}