using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleHelp : GameConsoleCommand
    {
        public override string commandName { get => "help"; }
        public override string description { get => "displays help"; }
        public override string help { get => "Use help; help <index>; help <command>"; }

        private int onePageCommandLimit = 5;
        private int maxPages;

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
            if (command.aliases != null)
            {
                aliasList = "\nCommand aliases:";
                for (int i = 0; i < command.aliases.Length; i++)
                    aliasList += $" {command.aliases[i]}";
            }
            Log($"Help for command <b>{command.commandName}</b>: {command.help}{aliasList}", "default");
        }

        private void CalculateMaxPages() => maxPages = (int)Mathf.Ceil((float)GameConsoleCommandList.GetList().Count / onePageCommandLimit);

        private void DisplayHelp(int pageIndex)
        {
            string helpMessage = $"<b>Help page {pageIndex} out of {maxPages}:</b>\n";
            List<GameConsoleCommand> commands = GameConsoleCommandList.GetList();

            for (int i = 0; i < onePageCommandLimit; i++)
            {
                if (commands.Count > pageIndex * onePageCommandLimit + i)
                {
                    GameConsoleCommand command = commands[pageIndex * onePageCommandLimit + i];
                    helpMessage += $"{command.commandName} - {command.description}\n";
                }
            }
            Log(helpMessage, "default");
        }
    }
}