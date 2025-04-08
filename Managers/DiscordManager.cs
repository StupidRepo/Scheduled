using System.Security.Cryptography;
using System.Text;
using BepInEx.Logging;
using Discord;
using MonoMod.Utils;
using Steamworks;
using LogLevel = Discord.LogLevel;
using Result = Discord.Result;

namespace Scheduled.Managers;

public class DiscordManager
{
	public const long CLIENT_ID = 1358163596479303941;
	public const string STATE = "Playing Schedule I";
	
	public readonly Discord.Discord? Discord;
	public readonly ActivityManager ActivityManager;
	
	private readonly ManualLogSource logger = Logger.CreateLogSource("Discord Game SDK");
	
	public DiscordManager()
	{
		Discord = new Discord.Discord(CLIENT_ID, (ulong)CreateFlags.NoRequireDiscord);
		Discord.SetLogHook(LogLevel.Debug, (level, message) =>
		{
			// ReSharper disable once ConvertIfStatementToSwitchStatement, thank you :3
			if (level == LogLevel.Warn) logger.LogWarning(message);
			else if (level == LogLevel.Error) logger.LogError(message);
			else logger.LogDebug(message);
		});
		
		ActivityManager = Discord.GetActivityManager();
		ActivityManager.OnActivityJoin += (lobbyIdString =>
		{
			if (!ulong.TryParse(lobbyIdString, out var lobbyId))
			{
				logger.LogError("Failed to parse lobby ID: " + lobbyIdString);
				return;
			}

			SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
		});
	}

	public void UpdateLobbyActivity(SteamLobby? lobby)
	{
		if (lobby == null)
		{
			UpdateActivity(Plugin.DEFAULT_ACTIVITY);
			return;
		}

		var lobbyId = lobby.Id.ToString();
		
		var sha256 = SHA256.Create();
		var hash = sha256.ComputeHash(Encoding.Unicode.GetBytes(lobbyId)).ToHexadecimalString();
		
		sha256.Dispose();
		
		var activity = Plugin.DEFAULT_ACTIVITY;
		activity.Party = new ActivityParty
		{
			Id = hash,
			Size =
			{
				CurrentSize = lobby.Players,
				MaxSize = lobby.MaxPlayers
			}
		};
		
		if(Plugin.Config.AllowInvites.Value)
			activity.Secrets = new ActivitySecrets { Join = lobbyId };
		
		UpdateActivity(activity);
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