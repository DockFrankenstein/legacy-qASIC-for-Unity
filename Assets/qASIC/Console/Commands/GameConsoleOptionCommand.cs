using System.Collections.Generic;
using qASIC.Options;

namespace qASIC.Console.Commands
{
    public class GameConsoleOptionCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().changeSettingCommand; }
        public override string CommandName { get; } = "changesetting";
        public override string Description { get; } = "changes the specified setting";
        public override string Help { get; } = "Use changesettings <setting name> <value>";
        public override string[] Aliases { get; } = new string[] { "changesettings", "settings", "setting", "option", "options", "changeoption" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 2)) return;

            if (!OptionsController.Enabled)
            {
                LogError("Options System is disabled!");
                return;
            }

            OptionsController.ChangeOption(args[1], args[2]);
        }
    }
}