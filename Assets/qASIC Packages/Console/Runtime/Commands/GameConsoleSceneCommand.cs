using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleSceneCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().sceneCommand; }
        public override string CommandName { get; } = "scene";
        public override string Description { get; } = "loads specified scene";
        public override string Help { get; } = "Use scene; scene <name>";
        public override string[] Aliases { get; } = new string[] { "loadscene", "level", "loadlevel" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 2)) return;
            switch (args.Count)
            {
                default:
                    LogScene();
                    return;
                case 2:
                    if (args[1].ToLower() == "reload")
                    {
                        ReloadScene();
                        return;
                    }

                    if (int.TryParse(args[1], out int sceneIndex))
                    {
                        LoadScene(sceneIndex);
                        return;
                    }

                    LoadScene(args[1]);
                    return;
            }
        }

        void LoadScene(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                LogError($"Scene '{sceneName}' does not exist!");
                return;
            }
            SceneManager.LoadScene(sceneName);
        }

        void LoadScene(int sceneIndex)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneIndex))
            {
                LogError("Index is out of range!");
                return;
            }
            SceneManager.LoadScene(sceneIndex);
        }

        void LogScene() => Log($"Current scene: '{SceneManager.GetActiveScene().name}'", "scene");

        void ReloadScene() => LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}