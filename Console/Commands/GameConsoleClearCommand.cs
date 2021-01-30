using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleClearCommand : GameConsoleCommand
    {
        public override string commandName { get => "clear"; }
        public override string description { get => "clears console"; }
        public override string help { get => "Clears console"; }
        public override string[] aliases { get => new string[] { "clr", "cls" }; }

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            GameConsoleController.Log(string.Empty, "default", Logic.GameConsoleLog.LogType.clear, false);
        }
    }
}