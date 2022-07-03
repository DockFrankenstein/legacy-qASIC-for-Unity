using System.Collections.Generic;
using qASIC.Console.Commands;
using UnityEngine;

namespace qASIC.Tests.Runtime
{
    public class TestCommand : GameConsoleCommand
    {
        public override bool Active => Application.isEditor;
        public override string CommandName { get; } = "test";
        public override string Description { get; } = "command used for testing qASIC. IT ONL EXISTS IN THE EDITOR!";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;

            if (args.Count == 1)
            {
                Log("[Test] Test command");
                return;
            }

            switch (args[1].ToLower())
            {
                case "exception":
                    throw new System.Exception("This is a test exception");
            }
        }
    }
}