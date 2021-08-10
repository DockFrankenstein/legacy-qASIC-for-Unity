using System.Collections.Generic;

namespace qASIC.Console.Commands
{
    public class GameConsoleEchoCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().echoCommand; }
        public override string CommandName { get; } = "echo";
        public override string Description { get; } = "creates a new log containing a message";
        public override string Help { get; } = "Write a message to echo";
        public override string[] Aliases { get; } = new string[] { "print" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCountMin(args, 1)) return;
            Log(System.Text.RegularExpressions.Regex.Replace(string.Join(" ", args.GetRange(1, args.Count - 1).ToArray()), "<.*?>", string.Empty), "default");
        }
    }
}