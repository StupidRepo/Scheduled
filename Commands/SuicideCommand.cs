using ScheduleOne.PlayerScripts;
using Console = ScheduleOne.Console;

namespace Scheduled.Commands;

public class SuicideCommand: Console.ConsoleCommand
{
    public override void Execute(List<string> args)
    {
        Player.Local.Health.SetHealth(0.0f);
    }

    public override string CommandWord => "suicide";
    public override string CommandDescription => "Kills you.";
    public override string ExampleUsage => "suicide";
}