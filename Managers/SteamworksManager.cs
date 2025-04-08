using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using ScheduleOne.Networking;
using ScheduleOne.Product;
using Steamworks;
using Logger = BepInEx.Logging.Logger;

namespace Scheduled.Managers;

public class SteamworksManager
{
	private readonly ManualLogSource logger = Logger.CreateLogSource("Steamworks Manager");
	
	public SteamworksManager()
	{
		if (!SteamAPI.Init())
		{
			throw new Exception("SteamAPI failed to initialize.");
		}
		
		logger.LogInfo("SteamAPI initialized successfully.");
		
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