using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleExitCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().exitCommand; }
        public override string CommandName { get; } = "exit";
        public override string Description { get; } = "closes the game";
        public override string Help { get; } = "Closes the game";
        public override string[] Aliases { get; } = new string[] { "quit" };

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