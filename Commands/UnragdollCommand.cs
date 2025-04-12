using ScheduleOne.PlayerScripts;
using Console = ScheduleOne.Console;

namespace Scheduled.Commands;

public class UnragdollCommand: Console.ConsoleCommand
{
    public override void Execute(List<string> args)
    {
        Player.Local.SetRagdolled(false);
    }

    public override string CommandWord => "unragdoll";
    public override string CommandDescription => "Unragdolls your character.";
    public override string ExampleUsage => "unragdoll";
}