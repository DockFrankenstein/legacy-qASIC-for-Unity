using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace qASIC.Console.Commands
{
    public class GameConsoleSceneListCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().sceneListCommand; }
        public override string CommandName { get; } = "scenelist";
        public override string Description { get; } = "displays list of all scenes";
        public override string Help { get; } = "Displays list of all scenes";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;

            string sceneList = "List of all scenes:";
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                sceneList += $"\n- {i}:{Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i))}";

            Log(sceneList, "info");
        }
    }
}