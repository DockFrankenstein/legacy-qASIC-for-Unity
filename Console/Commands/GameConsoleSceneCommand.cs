using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleSceneCommand : GameConsoleCommand
    {
        public override string commandName { get => "scene"; }
        public override string description { get => "get, load scene"; }
        public override string help { get => "Use get; load"; }

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 2)) return;
            switch (args.Count)
            {
                case 1:
                    LogScene();
                    break;
                case 2:
                    if (args[1].ToLower() == "get") LogScene();
                    else LoadScene(args[1]);
                    break;
                case 3:
                    if (args[1] == "load") LoadScene(args[2]);
                    break;
                default:
                    Log("There was an error while executing command <b>Scene</b>", "error");
                    break;
            }
        }

        private void LoadScene(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
                Log("Scene does not exist!", "Error1");
            SceneManager.LoadScene(sceneName);
        }

        private void LogScene() => Log($"Current scene: <b>{SceneManager.GetActiveScene().name}</b>", "Scene");
    }
}