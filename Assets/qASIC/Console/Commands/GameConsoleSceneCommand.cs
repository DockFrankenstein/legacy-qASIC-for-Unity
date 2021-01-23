using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleSceneCommand : GameConsoleCommand
    {
        public string commandName { get; set; }
        public string description { get; set; }

        public void Run(List<string> args)
        {
            if (!GameConsoleController.CheckForArgumentCount(args, 0, 1)) return;
            switch (args.Count)
            {
                case 0:
                    GameConsoleController.Log("Current scene: <b>" + SceneManager.GetActiveScene().name + "</b>", "Scene");
                    break;
                case 1:
                    if(!Application.CanStreamedLevelBeLoaded(args[1]))
                        GameConsoleController.Log("Scene does not exist!", "Error1");
                    LevelManager.LoadScene(args[1]);
                    GameConsoleController.Log("Loaded scene: <b>" + args[1] + "</b>", "Scene");
                    break;
            }
        }
    }
}