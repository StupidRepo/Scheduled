using HarmonyLib;
using ScheduleOne.ObjectScripts;
using UnityEngine;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(PackagingStation))]
public class PackagingStationPatches
{
	[HarmonyPatch(nameof(PackagingStation.Awake))]
	[HarmonyPostfix]
	public static void AwakePostfix(PackagingStation __instance)
	{
		if (__instance.ActiveProductAlignments.Length >= 50) return;
		// clone the elements of the array until the length is 50 (keep going until the end of the array, we can wrap by using modulus)
		var newArray = new Transform[50];
		for (var i = 0; i < newArray.Length; i++)
		{
			newArray[i] = __instance.ActiveProductAlignments[i % __instance.ActiveProductAlignments.Length];
		}
		__instance.ActiveProductAlignments = newArray;
	}
}