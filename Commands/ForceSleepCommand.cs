using ScheduleOne.DevUtilities;
using ScheduleOne.GameTime;
using ScheduleOne.PlayerScripts;
using Console = ScheduleOne.Console;

namespace Scheduled.Commands;

public class ForceSleepCommand: Console.ConsoleCommand
{
    public override void Execute(List<string> args)
    {
        TimeManager.Instance.StartSleep();
    }

    public override string CommandWord => "forcesleep";
    public override string CommandDescription => "Forces the game to progress to the next day. WARNING: May bug out other players in multiplayer. Press ESC when menu appears to interact with it.";
    public override string ExampleUsage => "forcesleep";
}