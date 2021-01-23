using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleEcho : GameConsoleCommand
    {
        public string commandName { get; set; }
        public string description { get; set; }

        public void Run(List<string> args)
        {
            if (GameConsoleController.CheckForArgumentCount(args, 1))
                GameConsoleController.Log(args[1], "Default");
        }
    }
}