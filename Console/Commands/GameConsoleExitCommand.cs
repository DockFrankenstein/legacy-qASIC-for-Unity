using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleExitCommand : GameConsoleCommand
    {
        public override string commandName { get => "exit"; }
        public override string description { get => "closes the game"; }
        public override string help { get => "Closes the game"; }
        public override string[] aliases { get => new string[] { "quit" }; }

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}