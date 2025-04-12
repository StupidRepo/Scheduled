using ScheduleOne.PlayerScripts;
using Console = ScheduleOne.Console;

namespace Scheduled.Commands;

public class RagdollCommand: Console.ConsoleCommand
{
    public override void Execute(List<string> args)
    {
        Player.Local.SetRagdolled(true);
    }

    public override string CommandWord => "ragdoll";
    public override string CommandDescription => "Ragdolls your character.";
    public override string ExampleUsage => "ragdoll";
}