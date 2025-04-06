using HarmonyLib;
using Scheduled.Commands;
using Console = ScheduleOne.Console;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(UnityEngine.Debug))]
public class PerformancePatches
{
    [HarmonyPatch(nameof(UnityEngine.Debug.LogWarning))]
    [HarmonyPrefix]
    public static bool LogWarning()
    {
        return false; // stop spamming the console with warnings
    }
    
    [HarmonyPatch(nameof(UnityEngine.Debug.LogError))]
    [HarmonyPostfix]
    public static bool LogError()
    {
        return false; // stop spamming the console with errors
    }
}