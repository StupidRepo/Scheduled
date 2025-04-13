using BepInEx.Configuration;
using BepInEx.Logging;
using Scheduled.Managers;
using ScheduleOne.Networking;

namespace Scheduled;

internal class ScheduledConfig
{
	private readonly ConfigFile config;
	private readonly ManualLogSource logger = Logger.CreateLogSource("Scheduled Config");
	
	// Discord
	internal ConfigEntry<bool> InteractWithDiscord;
	internal ConfigEntry<bool> AllowInvites;
	internal ConfigEntry<bool> ShowPublicWarning;
	
	// Tweaks
	internal ConfigEntry<int> StackSizeMultiplier;
	internal ConfigEntry<bool> ImprovePerformance;
	
	// Steam Game Server
	internal ConfigEntry<bool> DedicatedServerMode;
	internal ConfigEntry<string> ServerLoginToken;
	
	internal ConfigEntry<string> ServerName;
	internal ConfigEntry<int> MaxPlayers;
	
	internal ScheduledConfig(ConfigFile config)
	{
		this.config = config;
		LoadValues();
	}

	private void LoadValues()
	{
		// Discord
		InteractWithDiscord = config.Bind(
			GetName(Sections.Discord),
			nameof(InteractWithDiscord), 
			true,
			"Whether or not to interact with Discord, for enabling RPC and game invites."
		);
		AllowInvites = config.Bind(
			GetName(Sections.Discord),
			nameof(AllowInvites), 
			true,
			"Whether or not to enable invite support."
		);
		ShowPublicWarning = config.Bind(
			GetName(Sections.Discord),
			nameof(ShowPublicWarning), 
			true,
			"Whether or not to show a warning for game invites making lobbies public."
		);
		if(InteractWithDiscord.Value == false)
			AllowInvites.Value = false;

		// Tweaks
		StackSizeMultiplier = config.Bind(
			GetName(Sections.Tweaks),
			nameof(StackSizeMultiplier), 
			4,
			"Multiplies the maximum stack size for items. New max = original max * multiplier. The multiplier will not go below 1."
		);
		ImprovePerformance = config.Bind(
			GetName(Sections.Tweaks),
			nameof(ImprovePerformance), 
			true,
			"Improves performance by disabling mass logging of FishNet. " +
			"It's recommended to keep this on, especially when making bug reports that require your Player.log file."
		);
		
		// Steam Game Server
		DedicatedServerMode = config.Bind(
			GetName(Sections.SteamGameServer),
			nameof(DedicatedServerMode), 
			false,
			"Whether or not to run the game as a dedicated server."
		);
		ServerLoginToken = config.Bind(
			GetName(Sections.SteamGameServer),
			nameof(ServerLoginToken), 
			string.Empty,
			"Login token for the dedicated server." +
			"Open this link in your browser to create a token (will open in Steam app): steam://openurl/https://steamcommunity.com/dev/managegameservers"
		);
		
		ServerName = config.Bind(
			GetName(Sections.SteamGameServer),
			nameof(ServerName), 
			"Schedule I Dedicated Server",
			"Name of the server. This will be shown in the server list."
		);
		MaxPlayers = config.Bind(
			GetName(Sections.SteamGameServer),
			nameof(MaxPlayers), 
			32,
			"Maximum number of players allowed on the server."
		);
		
		logger.LogInfo("✅ Config loaded!");
	}
	
	private string GetName(Sections section)
	{
		return Enum.GetName(typeof(Sections), section) ?? "Unknown";
	}
}

internal enum Sections
{
	Discord,
	Tweaks,
	SteamGameServer
}