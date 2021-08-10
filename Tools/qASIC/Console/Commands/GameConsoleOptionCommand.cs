using System.Collections.Generic;
using qASIC.Options;

namespace qASIC.Console.Commands
{
    public class GameConsoleOptionCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().optionCommand; }
        public override string CommandName { get; } = "changeoption";
        public override string Description { get; } = "changes basic options";
        public override string Help { get; } = "Use changeoption <setting name> <value>";
        public override string[] Aliases { get; } = new string[] { "option", "options", "settings" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 2)) return;
            OptionsController.ChangeOption(args[1], args[2]);
        }
    }
}