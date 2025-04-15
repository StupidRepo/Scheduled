using BepInEx.Logging;
using ScheduleOne.UI.MainMenu;
using Steamworks;
using UnityEngine;
using Logger = BepInEx.Logging.Logger;

namespace Scheduled.Managers;

public class GameServerManager
{
	private readonly ManualLogSource logger = Logger.CreateLogSource("GameServerManager");
	public readonly bool IsInit;
	public bool IsRunning;
	
	private const int GAMEPLAY_PORT = 27015;
	private const int QUERY_PORT = 27016;
	
	public GameServerManager()
	{
		if (!GameServer.Init(0, GAMEPLAY_PORT, QUERY_PORT, EServerMode.eServerModeAuthentication, Application.version))
		{
			logger.LogError("Game server failed to initialise.");
			if(MainMenuPopup.InstanceExists)
				MainMenuPopup.Instance.Open("Error", "Game server failed to initialise.", true);
			
			return;
		}
		
		logger.LogInfo($"Game server created for ports :{QUERY_PORT} and :{GAMEPLAY_PORT}.");
		IsInit = true;
		
		Callback<SteamServersConnected_t>.CreateGameServer(_ =>
		{
			IsRunning = true;
			
			logger.LogWarning($"Game server is now connected to Steam servers with a Steam ID of {SteamGameServer.GetSteamID().m_SteamID}!");
			if (MainMenuPopup.InstanceExists)
				MainMenuPopup.Instance.Open("Info", "The game server is now connected to Steam servers.", false);
			
			SteamGameServer.SetAdvertiseServerActive(true);
			UpdateInfo();
		});
		Callback<SteamServerConnectFailure_t>.CreateGameServer(f =>
		{
			logger.LogError("Failed to login to Steam servers: " + f.m_eResult);
			if (MainMenuPopup.InstanceExists)
				MainMenuPopup.Instance.Open("Error", $"Failed to login to Steam servers, did you supply the right token? ({f.m_eResult.ToString()})", true);
		});
		Callback<SteamServersDisconnected_t>.CreateGameServer(f =>
		{
			IsRunning = false;
			
			logger.LogError("GS got disconnected from Steam servers: " + f.m_eResult);
			if (MainMenuPopup.InstanceExists)
				MainMenuPopup.Instance.Open("Error", $"Game server was disconnected from Steam servers: {f.m_eResult.ToString()}", true);
		});
		
		SteamApps.GetAppInstallDir(Plugin.AppId, out var installDir, 2048);
		logger.LogInfo($"App install dir: {installDir}");
		
		if (!Directory.Exists(installDir))
		{
			return; // could be a lil pirate, let's block 'em hehe
		}
		
		SteamGameServer.SetProduct("Schedule I");
		SteamGameServer.SetGameDescription("Schedule I");
		
		SteamGameServer.SetModDir(
			"s1_dedicated_server"
		);
		
		logger.LogInfo($"Logging in to Steam Game Server...");
		SteamGameServer.LogOn(Plugin.Config.ServerLoginToken.Value);
	}

	public void UpdateInfo()
	{
		SteamGameServer.SetServerName(Plugin.Config.ServerName.Value);
		SteamGameServer.SetMapName("Main Map");
		
		SteamGameServer.SetMaxPlayerCount(Math.Min(Math.Max(Plugin.Config.MaxPlayers.Value, 1), 128));
		
		SteamGameServer.SetPasswordProtected(false);
		SteamGameServer.SetDedicatedServer(true);
	}
}