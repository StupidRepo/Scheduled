using BepInEx.Configuration;
using BepInEx.Logging;
using Scheduled.Managers;
using ScheduleOne.Networking;
using ScheduleOne.UI;

namespace Scheduled;

internal class ScheduledConfig
{
	private readonly ConfigFile config;
	private readonly ManualLogSource logger = Logger.CreateLogSource("Scheduled Config");
	
	// Discord
	internal ConfigEntry<bool> InteractWithDiscord;
	internal ConfigEntry<bool> AllowInvites;
	
	// Tweaks
	internal ConfigEntry<int> StackSizeMultiplier;
	internal ConfigEntry<bool> ImprovePerformance;
	
	internal ScheduledConfig(ConfigFile config)
	{
		this.config = config;
		
		config.ConfigReloaded += (_, _) =>
		{
			logger.LogInfo("🔃 Reloading config...");
			LoadValues();
		};
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

		if (AllowInvites.Value && InteractWithDiscord.Value && Lobby.InstanceExists)
		{
			logger.LogInfo("🕹️ Updating Discord activity, due to config reload...");
			Plugin.DiscordManager?.UpdateLobbyActivity(new SteamLobby(Lobby.Instance.LobbyID));
		}
		else if (!AllowInvites.Value || !InteractWithDiscord.Value)
		{
			Plugin.DiscordManager?.UpdateLobbyActivity(null);
		}
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
	Tweaks
}