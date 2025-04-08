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
        __result *= Math.Min(Plugin.Config.StackSizeMultiplier.Value, 1);
    }
}