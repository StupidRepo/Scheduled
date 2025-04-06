using HarmonyLib;
using Scheduled.Commands;
using Console = ScheduleOne.Console;

namespace Scheduled.Patches;

[HarmonyPatch(typeof(Console))]
public class ConsolePatches
{
    [HarmonyPatch(nameof(Console.Awake))]
    [HarmonyPostfix]
    public static void AwakePostfix(Console __instance)
    {
        Plugin.Logger.LogInfo("Adding custom commands!");
        
        Console.commands.Add("suicide", new SuicideCommand());
        Console.commands.Add("forcesleep", new ForceSleepCommand());
    }
}