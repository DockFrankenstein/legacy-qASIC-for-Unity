using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleEcho : GameConsoleCommand
    {
        public override string CommandName { get; } = "echo";
        public override string Description { get; } = "creates a new log containing a message";
        public override string Help { get; } = "Write a message to echo";
        public override string[] Aliases { get; } = new string[] { "print" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 1)) return;
            string log = args[1];
            for (int i = 0; i < args.Count - 2; i++) log += $" {args[i + 2]}";
            Log(log, "default");
        }
    }
}