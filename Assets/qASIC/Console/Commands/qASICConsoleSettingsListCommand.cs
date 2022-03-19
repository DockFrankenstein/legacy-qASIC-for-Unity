using System.Linq;
using System.Collections.Generic;
using System.Text;
using qASIC.Options;

namespace qASIC.Console.Commands
{
    public class qASICConsoleSettingsListCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().settingListCommand; }
        public override string CommandName { get; } = "settinglist";
        public override string Description { get; } = "lists all settings";
        public override string[] Aliases { get; } = new string[] { "settingslist", "listsettings" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            StringBuilder log = new StringBuilder("Avaliable settings:");
            List<string> settings = OptionsController.GetSettingNames().Distinct().ToList();
            for (int i = 0; i < settings.Count; i++)
                log.Append($"\n- {settings[i]}");

            Log(log.ToString(), "info");
        }
    }
}