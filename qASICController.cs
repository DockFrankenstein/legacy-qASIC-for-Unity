using qASIC.InputManagment;
using qASIC.Console;
using UnityEngine;

namespace qASIC
{
    public class qASICController : MonoBehaviour
    {
        private static bool hasStarted = false;

        private void Awake()
        {
            if (hasStarted) return;
            SetConfig();
            hasStarted = true;
        }

        private void LogLoadedScene(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) =>
            GameConsoleController.Log($"Loaded scene {scene.name}", "scene", Console.Logic.GameConsoleLog.LogType.game, false);

        private void SetConfig()
        {
            if (!GameConsoleController.TryGettingConfig(out Console.Tools.GameConsoleConfig config)) return;
            if(config.showInputMessages) LogInput();
            if(config.logScene) UnityEngine.SceneManagement.SceneManager.sceneLoaded += LogLoadedScene;
        }

        private void LogInput() =>
            Console.Commands.GameConsoleInputCommand.Print();
    }
}