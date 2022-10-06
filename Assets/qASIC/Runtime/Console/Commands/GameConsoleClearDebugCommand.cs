using System.Collections.Generic;
using qASIC.Displayer;

namespace qASIC.Console.Commands
{
    public class GameConsoleClearDebugCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().clearDebugDisplayerCommand; }
        public override string CommandName { get; } = "cleardebugdisplayer";
        public override string Description { get; } = "clears debug displayer";
        public override string Help { get; } = "Clears debug displayer";
        public override string[] Aliases { get; } = new string[] { "cleardebug" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            InfoDisplayer.ClearDisplayer("debug");
            Log("Debug displayer has been cleaned", "info");
        }
    }
}