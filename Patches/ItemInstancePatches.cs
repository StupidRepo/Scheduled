using HarmonyLib;
using ScheduleOne.ItemFramework;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(ItemInstance))]
public class ItemInstancePatches
{
    [HarmonyPatch(nameof(ItemInstance.StackLimit), MethodType.Getter)]
    [HarmonyPostfix]
    public static void StackLimitOverride(ItemInstance __instance, ref int __result)
    {
        Plugin.Logger.LogError("Original stack limit for " + __instance.Name + ": " + __result);
        __result *= 4;
    }
}