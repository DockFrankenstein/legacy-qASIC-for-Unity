using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleSceneCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().sceneCommand; }
        public override string CommandName { get; } = "scene";
        public override string Description { get; } = "get, load scene";
        public override string Help { get; } = "Use scene; scene get; scene load <scene name>";
        public override string[] Aliases { get; } = new string[] { "loadscene", "level", "loadlevel" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 2)) return;
            switch (args.Count)
            {
                case 1:
                    LogScene();
                    break;
                case 2:
                    switch (args[1].ToLower())
                    {
                        case "get":
                            LogScene();
                            break;
                        case "reload":
                            LoadScene(SceneManager.GetActiveScene().name);
                            break;
                        default:
                            if (!Application.CanStreamedLevelBeLoaded(args[1]))
                            {
                                LogError($"Scene <b>{args[1]}</b> does not exist!");
                                return;
                            }
                            LoadScene(args[1]);
                            break;
                    }
                    break;
                case 3:
                    if (args[1] == "load") LoadScene(args[2]);
                    break;
                default:
                    LogError("There was an error while executing command <b>Scene</b>");
                    break;
            }
        }

        private void LoadScene(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                LogError("Scene does not exist!");
                return;
            }
            SceneManager.LoadScene(sceneName);
        }

        private void LogScene() => Log($"Current scene: <b>{SceneManager.GetActiveScene().name}</b>", "scene");
    }
}