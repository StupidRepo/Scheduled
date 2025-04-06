using GameKit.Utilities.Types;
using HarmonyLib;
using ItemIconCreator;
using ScheduleOne.DevUtilities;
using ScheduleOne.Economy;
using ScheduleOne.Quests;
using ScheduleOne.StationFramework;
using ScheduleOne.UI.Compass;
using UnityEngine;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(SupplierLocationConfiguration))]
public class SupplierConfigPatches
{
    [HarmonyPatch(nameof(SupplierLocationConfiguration.Activate))]
    [HarmonyPrefix]
    public static void ActivatePrefix(SupplierLocationConfiguration __instance)
    {
        Plugin.Logger.LogInfo($"Activating compass for supplier: {__instance.SupplierID}");
        CompassManager.Instance.AddElement(
            __instance.gameObject.transform,
            Resources.LoadAll<RectTransform>("").First()
        );
    }

    [HarmonyPatch(nameof(SupplierLocationConfiguration.Deactivate))]
    [HarmonyPrefix]
    public static void DeactivatePrefix(SupplierLocationConfiguration __instance)
    {
        Plugin.Logger.LogInfo($"Deactivating compass for supplier: {__instance.SupplierID}");
        CompassManager.Instance.RemoveElement(
            __instance.gameObject.transform
        );
    }
}