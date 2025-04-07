using BepInEx.Logging;
using Discord;
using Steamworks;
using LogLevel = Discord.LogLevel;
using Result = Discord.Result;

namespace Scheduled.Managers;

public class DiscordManager
{
	public const string STATE = "Playing Schedule I";
	
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
	}

	public void UpdateActivity(string details)
	{
		var activityManager = Discord.GetActivityManager();
		var activity = new Activity
		{
			State = STATE,
			Details = details
		};

		activityManager.UpdateActivity(activity, result =>
		{
			logger.LogInfo(result == Result.Ok
				? "Activity updated successfully."
				: $"Failed to update activity: {result}");
		});
	}

	public void UpdateActivity(ulong lobbyId)
	{
		
	}
}

internal class SteamLobby
{
	public ulong LobbyId { get; private set; }
	public bool IsPublic { get; private set; }

	public int CurrentPlayers { get; private set; }
	public int MaxPlayers { get; private set; }
	
	public SteamLobby(ulong lobbyId)
	{
		// if steamworks not init, throw
		if (!SteamClient.IsValid) throw new InvalidOperationException("Bro what are you doing? Steamworks is not initialized!");
		
		LobbyId = lobbyId;
	}
}