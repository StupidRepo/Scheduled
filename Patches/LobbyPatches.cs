// using HarmonyLib;
// using ScheduleOne.Networking;
//
// namespace Scheduled.Patches;
//
// [HarmonyPatch(typeof(Lobby))]
// public class LobbyPatches
// {
// 	[HarmonyPatch(nameof(Lobby.InitializeCallbacks))]
// 	[HarmonyPrefix]
// 	public static bool GoodbyeInitCallbacks()
// 	{
// 		return false;
// 	}
//
// 	[HarmonyPatch(nameof(Lobby.LeaveLobby))]
// 	[HarmonyPostfix]
// 	public static void LeaveLobbyHook()
// 	{
// 		Plugin.DiscordManager.UpdateLobbyActivity(null);
// 	}
// }