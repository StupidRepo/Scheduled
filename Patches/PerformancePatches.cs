using HarmonyLib;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(UnityEngine.Debug))]
public class PerformancePatches
{
    [HarmonyPatch(nameof(UnityEngine.Debug.LogWarning), typeof(object), typeof(UnityEngine.Object))]
    [HarmonyPrefix]
    public static bool LogWarning(object message, UnityEngine.Object context)
    {
        return false; // stop spamming the console with warnings
    }
    
    [HarmonyPatch(nameof(UnityEngine.Debug.LogError), typeof(object), typeof(UnityEngine.Object))]
    [HarmonyPrefix]
    public static bool LogError(object message, UnityEngine.Object context)
    {
        return false; // stop spamming the console with errors
    }
    
    [HarmonyPatch(nameof(UnityEngine.Debug.LogWarning), typeof(object), typeof(UnityEngine.Object))]
    [HarmonyPrefix]
    public static bool LogWarning(object message)
    {
        return false; // stop spamming the console with warnings
    }
    
    [HarmonyPatch(nameof(UnityEngine.Debug.LogError), typeof(object), typeof(UnityEngine.Object))]
    [HarmonyPrefix]
    public static bool LogError(object message)
    {
        return false; // stop spamming the console with errors
    }
}