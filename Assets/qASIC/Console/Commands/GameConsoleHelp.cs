using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleHelp : GameConsoleCommand
    {
        public override string commandName { get => "help"; }
        public override string description { get => "displays help"; }
        public override string help { get => "Use help; help <page index>; help <command>"; }

        private int onePageCommandLimit = 5;
        private int maxPages;

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;
            if (args.Count == 1)
                TryHelp(0);
            else if (int.TryParse(args[1], out int index))
                TryHelp(index);
            else if (TryGettingCommand(args[1], out GameConsoleCommand command))
                DsplayCommand(command);
            else
                Log("Command does not exist!", "error");
        }

        private bool TryGettingCommand(string commandName, out GameConsoleCommand command)
        {
            command = null;
            for (int i = 0; i < GameConsoleCommandList.commands.Count; i++)
                if (GameConsoleCommandList.commands[i].commandName == commandName)
                    command = GameConsoleCommandList.commands[i];
            return command != null;
        }

        private void TryHelp(int pageIndex)
        {
            CalculateMaxPages();
            if (pageIndex < maxPages)
                DisplayHelp(pageIndex);
            else
                Log("Page is out of range!", "error");
        }

        private void DsplayCommand(GameConsoleCommand command) => Log($"Help for command <b>{command.commandName}</b>: {command.help}", "default");

        private void CalculateMaxPages() => maxPages = (int)Mathf.Ceil((float)GameConsoleCommandList.commands.Count / onePageCommandLimit);

        private void DisplayHelp(int pageIndex)
        {
            string helpMessage = $"<b>Help page {pageIndex}:</b>\n";

            for (int i = 0; i < onePageCommandLimit; i++)
                if (GameConsoleCommandList.commands.Count > pageIndex * onePageCommandLimit + i)
                {
                    GameConsoleCommand command = GameConsoleCommandList.commands[pageIndex * onePageCommandLimit + i];
                    helpMessage += $"{command.commandName} - {command.description}\n";
                }

            Log(helpMessage, "default");
        }
    }
}