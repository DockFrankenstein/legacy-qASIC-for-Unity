using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleHelp : GameConsoleCommand
    {
        public string commandName { get; set; }
        public string description { get; set; }

        private int onePageCommandLimit = 5;
        private int maxPages;

        private void CalculateMaxPages() => 
            maxPages = (int)Mathf.Ceil((float)GameConsoleCommandList.commands.Length / onePageCommandLimit);

        public void Run(List<string> args)
        {
            CalculateMaxPages();

            if (GameConsoleController.CheckForArgumentCount(args, 0, 1))
            {
                if (args.Count == 1)
                    TryHelp(0);
                else if (int.TryParse(args[1], out int index))
                    TryHelp(index);
                else
                    GameConsoleController.Log("Couldn't parse index!", "Error2");
            }
        }

        private void TryHelp(int pageIndex)
        {
            if (pageIndex < maxPages)
                DisplayHelp(pageIndex);
            else
                GameConsoleController.Log("Page is out of range!", "Error1");
        }

        private void DisplayHelp(int pageIndex)
        {
            string helpMessage = "<b>Help page " + pageIndex + ":</b>\n";

            for (int i = 0; i < onePageCommandLimit; i++)
                if (GameConsoleCommandList.commands.Length > pageIndex * onePageCommandLimit + i)
                {
                    GameConsoleCommand command = GameConsoleCommandList.commands[pageIndex * onePageCommandLimit + i];
                    helpMessage += command.commandName + " - " + command.description + "\n";
                }

            GameConsoleController.Log(helpMessage, "Default");
        }
    }
}