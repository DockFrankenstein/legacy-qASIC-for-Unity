using System.Collections.Generic;
using qASIC.Input;

namespace qASIC.Console.Commands
{
    public class GameConsoleInputSaveCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "savecablebox";
        public override string Description { get; } = "Saves player preferences of Cablebox Input System";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            InputManager.SavePreferences();
        }
    }
}