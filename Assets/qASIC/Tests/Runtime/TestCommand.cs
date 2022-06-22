using System.Collections.Generic;
using qASIC.Console.Commands;

namespace qASIC.Tests.Runtime
{
    public class TestCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "test";
        public override string Description { get; } = "command used for testing";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 1)) return;

            switch (args[1].ToLower())
            {
                case "exception":
                    throw new System.Exception("This is a test exception");
            }
        }
    }
}