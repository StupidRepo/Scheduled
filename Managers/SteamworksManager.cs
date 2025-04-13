using BepInEx.Logging;
using IO.Swagger.Model;
using MonoMod.RuntimeDetour;
using ScheduleOne.Networking;
using ScheduleOne.UI.MainMenu;
using Steamworks;
using Application = UnityEngine.Application;
using Logger = BepInEx.Logging.Logger;

namespace Scheduled.Managers;

public class SteamworksManager
{
	private readonly ManualLogSource logger = Logger.CreateLogSource("Steamworks Manager");
	private readonly ManualLogSource gsLogger = Logger.CreateLogSource("GameServer");
	public bool IsInit;

	public SteamworksManager()
	{
		if (!SteamAPI.Init())
		{
			throw new Exception("SteamAPI failed to initialise.");
		}

		IsInit = true;
		logger.LogInfo("SteamAPI initialised successfully.");

		Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		Callback<LobbyEnter_t>.Create(OnLobbyEntered);
		Callback<LobbyChatUpdate_t>.Create(OnMemberChanged);

		new Hook(
			typeof(Lobby).GetMethod(nameof(Lobby.LeaveLobby))!,
			(Action<Lobby> orig, Lobby self) =>
			{
				orig(self);

				logger.LogDebug("Updating activity!");
				Plugin.DiscordManager?.UpdateLobbyActivity(null);
			}
		).Apply();
	}
	
	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult != EResult.k_EResultOK && (Plugin.Config.AllowInvites.Value && Plugin.Config.InteractWithDiscord.Value)) return;
		
		logger.LogWarning("Made lobby public!");
				
		if(MainMenuPopup.InstanceExists && Plugin.Config.ShowPublicWarning.Value)
		{
			MainMenuPopup.Instance.Open("Warning!",
				"This lobby was made public to allow seamless Discord invite integration with people you haven't added on Steam. " +
				"To turn this off, please disable the 'Allow Invites' option in the 'Scheduled' config file." +
				"\n\nThis warning will not show again.",
				true
			);
			Plugin.Config.ShowPublicWarning.Value = false;
		}

		var lobby = new CSteamID(callback.m_ulSteamIDLobby);
		
		SteamMatchmaking.SetLobbyType(lobby, ELobbyType.k_ELobbyTypePublic);
	}

	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		if (callback.m_EChatRoomEnterResponse != 1)
		{
			logger.LogError("Failed to enter lobby: " + callback.m_EChatRoomEnterResponse);
			Utils.ShowError("Failed to connect", $"Failed to connect to lobby: {callback.m_EChatRoomEnterResponse}");
			return;
		}
		
		var lobby = new SteamLobby(callback.m_ulSteamIDLobby);
		Plugin.DiscordManager?.UpdateLobbyActivity(lobby);
	}
	
	private void OnMemberChanged(LobbyChatUpdate_t callback)
	{
		if (callback.m_ulSteamIDLobby == 0)
		{
			logger.LogError("Lobby ID is 0.");
			Plugin.DiscordManager?.UpdateLobbyActivity(null);
			return;
		}

		var lobby = new SteamLobby(callback.m_ulSteamIDLobby);
		if (callback.m_rgfChatMemberStateChange == (ulong)EChatMemberStateChange.k_EChatMemberStateChangeEntered)
		{
			logger.LogInfo($"{callback.m_ulSteamIDUserChanged} joined the lobby.");
			Plugin.DiscordManager?.UpdateLobbyActivity(lobby);
		}
		else if (callback.m_rgfChatMemberStateChange == (ulong)EChatMemberStateChange.k_EChatMemberStateChangeLeft)
		{
			logger.LogInfo($"{callback.m_ulSteamIDUserChanged} left the lobby.");
			Plugin.DiscordManager?.UpdateLobbyActivity(lobby);
		}
		else
		{
			logger.LogDebug($"Member changed: {callback.m_ulSteamIDUserChanged}, state: {callback.m_rgfChatMemberStateChange}");
		}
	}
}

public class SteamLobby
{
	public CSteamID Id { get; private set; }
	public CSteamID Owner { get; private set; }
	public int Players { get; private set; }
	public int MaxPlayers { get; private set; }

	public SteamLobby(ulong lobbyId)
	{
		Id = new CSteamID(lobbyId);
		Owner = SteamMatchmaking.GetLobbyOwner(Id);
		
		Players = SteamMatchmaking.GetNumLobbyMembers(Id);
		MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit(Id);
	}
}