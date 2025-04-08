using FishNet.Managing.Logging;
using HarmonyLib;

namespace Scheduled.Patches;


public class PerformancePatches
{
    [HarmonyPatch(typeof(UnityEngine.Debug))]
    public class UnitySh_t
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
    
    [HarmonyPatch(typeof(LevelLoggingConfiguration))]
    public class FishNetSh_t // f*ck you fishnet devs. delete your docs, delete your networking 'solution'
                             // (lets be real though, it's a networking PROBLEM, not a solution you f*cking numbnuts) <-- how to get GitHub Copilot to not generate text
    {
        [HarmonyPatch(nameof(LevelLoggingConfiguration.CanLog))]
        [HarmonyPrefix]
        public static bool TurnOffLoggingF_ckOffFishNetGivingMe600MBLogsYouF_cker(ref bool __result)
        {
            __result = false; // step 1 to fixing performance in Schedule I
            return false; // step 2
            // success! :D
        }
    }
}