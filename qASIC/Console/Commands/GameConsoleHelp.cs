using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleHelp : GameConsoleCommand
    {
        public override string CommandName { get; } = "help";
        public override string Description { get; } = "displays help";
        public override string Help { get; } = "Use help; help <index>; help <command>";

        int onePageCommandLimit = 5;
        int maxPages;

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;
            if (args.Count == 1) TryHelp(0);
            else if (int.TryParse(args[1], out int index)) TryHelp(index);
            else if (GameConsoleCommandList.TryGettingCommandByName(args[1], out GameConsoleCommand command))
                DisplayCommand(command);
            else Log("Command does not exist!", "error");
        }

        private void TryHelp(int pageIndex)
        {
            CalculateMaxPages();
            if (pageIndex < maxPages) DisplayHelp(pageIndex);
            else Log("Page is out of range!", "error");
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
            Log($"Help for command <b>{command.CommandName}</b>: {command.Help}{aliasList}", "default");
        }

        private void CalculateMaxPages() => maxPages = (int)Mathf.Ceil((float)GameConsoleCommandList.Commands.Count / onePageCommandLimit);

        private void DisplayHelp(int pageIndex)
        {
            string helpMessage = $"<b>Help page {pageIndex + 1} out of {maxPages}:</b>\n";
            List<GameConsoleCommand> commands = GameConsoleCommandList.Commands;

            for (int i = 0; i < onePageCommandLimit; i++)
            {
                if (commands.Count > pageIndex * onePageCommandLimit + i)
                {
                    GameConsoleCommand command = commands[pageIndex * onePageCommandLimit + i];
                    helpMessage += $"{command.CommandName} - {command.Description}\n";
                }
            }
            Log(helpMessage, "default");
        }
    }
}