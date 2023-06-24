using System.Collections.Generic;
using qASIC.Input;

namespace qASIC.Console.Commands
{
    public class GameConsoleInputLoadCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "loadcablebox";
        public override string Description { get; } = "Loads player preferences of Cablebox Input System";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            InputManager.LoadPreferences();
        }
    }
}