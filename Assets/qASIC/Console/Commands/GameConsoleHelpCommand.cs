using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleHelpCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().helpCommand; }
        public override string CommandName { get; } = "help";
        public override string Description { get; } = "displays help";
        public override string Help { get; } = "Use help; help <index>; help <command>";

        int onePageCommandLimit = 5;
        bool useLimit = true;
        bool useDetail = true;
        int maxPages;

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;

            if (GameConsoleController.TryGettingConfig(out Tools.GameConsoleConfig config))
            {
                onePageCommandLimit = config.pageCommandLimit;
                useLimit = config.usePageCommandLimit;
                useDetail = config.showDetailedHelp;
            }

            if (args.Count == 1) TryHelp(0);
            else if (int.TryParse(args[1], out int index) && useLimit) TryHelp(index);
            else if (GameConsoleCommandList.TryGettingCommandByName(args[1], out GameConsoleCommand command) && useDetail)
                DisplayCommand(command);
            else LogError(useDetail ? "Command does not exist!" : "User input - index is out of range!");
        }

        private void TryHelp(int pageIndex)
        {
            CalculateMaxPages();
            if (pageIndex < maxPages)
            {
                DisplayHelp(pageIndex);
                return;
            }

            LogError("Page is out of range!");
        }

        private void DisplayCommand(GameConsoleCommand command)
        {
            string aliasList = "";
            if (command.Aliases != null)
            {
                aliasList = "\nCommand aliases:";
                for (int i = 0; i < command.Aliases.Length; i++)
                    aliasList += $" {command.Aliases[i]}";
            }
            string helpMessage = command.Help == null ? $"No help avalible for command <b>{command.CommandName}</b>" 
                : $"Help for command <b>{(command.CommandName)}</b>: {command.Help}";

            Log($"{helpMessage}{aliasList}", "info");
        }

        private void CalculateMaxPages()
        {
            if (useLimit)
            {
                maxPages = (int)Mathf.Ceil((float)GameConsoleCommandList.Commands.Count / onePageCommandLimit);
                return;
            }
            maxPages = 1;
        }

        private void DisplayHelp(int pageIndex)
        {
            string helpMessage = useLimit ? $"<b>Help page {pageIndex + 1} out of {maxPages}:</b>\n" : "<b>Avalible commands:</b>\n";
            List<GameConsoleCommand> commands = GameConsoleCommandList.Commands;

            int limit = useLimit ? onePageCommandLimit : commands.Count;

            for (int i = 0; i < limit; i++)
            {
                if (commands.Count <= pageIndex * limit + i) continue;
                GameConsoleCommand command = commands[pageIndex * limit + i];
                helpMessage += $"{command.CommandName} - {command.Description}\n";
            }
            Log(helpMessage, "info");
        }
    }
}