using HarmonyLib;
using ScheduleOne.DevUtilities;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using UnityEngine;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(NPCInventory))]
public class NPCInventoryPatches
{
	[HarmonyPatch(nameof(NPCInventory.CanPickpocket))]
	[HarmonyPrefix]
	public static bool CanPickpocketOverride(NPCInventory __instance, ref bool __result)
	{
		__result = __instance.CanBePickpocketed && 
		           PlayerSingleton<PlayerMovement>.Instance.isCrouched 
		           && Player.Local.CrimeData.CurrentPursuitLevel == PlayerCrimeData.EPursuitLevel.None 
		           && Time.time - __instance.timeOnLastExpire >= (!__instance.npc.IsConscious ? .5f : 30f)
		           && !__instance.npc.Health.IsDead
		           && !__instance.npc.behaviour.CallPoliceBehaviour.Active 
		           && !__instance.npc.behaviour.CombatBehaviour.Active 
		           && !__instance.npc.behaviour.FacePlayerBehaviour.Active 
		           && !__instance.npc.behaviour.FleeBehaviour.Active 
		           && !__instance.npc.behaviour.GenericDialogueBehaviour.Active 
		           && !__instance.npc.behaviour.StationaryBehaviour.Active 
		           && !__instance.npc.behaviour.RequestProductBehaviour.Active
		           && !GameManager.IS_TUTORIAL;
		return false;
	}
}