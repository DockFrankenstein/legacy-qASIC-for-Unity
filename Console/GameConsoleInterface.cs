using UnityEngine;
using TMPro;
using qASIC.Console.Tools;
using System.Collections;
using UnityEngine.Events;

namespace qASIC.Console
{
    public class GameConsoleInterface : MonoBehaviour
    {
        [Header("Settings")]
        public int LogLimit = 64;
        public GameConsoleConfig ConsoleConfig;

        [Header("Objects")]
        public GameObject CanvasObject;
        public TextMeshProUGUI Logs;
        public TMP_InputField Input;
        public UnityEngine.UI.ScrollRect Scroll;

        [Header("Events")]
        public UnityEventBool OnConsoleChangeState;

        private int _commandIndex = -1;

        public RuntimePlatform[] IgnoreReselectPlatforms = new RuntimePlatform[]
            {
                RuntimePlatform.IPhonePlayer,
                RuntimePlatform.Android,
            };

        private void Awake()
        {
            AssignConfig();
            ReloadInterface();
            SetupConfig();
            AddLogEvent();
        }

        /// <summary>Enables features like logging messages to console, or logging the scene</summary>
        private void SetupConfig()
        {
            if (ConsoleConfig == null) return;
            if (ConsoleConfig.LogScene) UnityEngine.SceneManagement.SceneManager.sceneLoaded += LogLoadedScene;
            Application.logMessageReceived += (string log, string trace, LogType type) => HandleUnityLog(log, trace, type);
        }

        private void HandleUnityLog(string logText, string trace, LogType type)
        {
            string color = "default";
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                case LogType.Assert:
                    if (!ConsoleConfig.LogUnityErrorsToConsole) return;
                    color = "error";
                    break;
                case LogType.Warning:
                    if (!ConsoleConfig.LogUnityWarningsToConsole) return;
                    color = "warning";
                    break;
                case LogType.Log:
                    if (!ConsoleConfig.LogUnityMessagesToConsole) return;
                    color = "default";
                    break;
            }

            Logic.GameConsoleLog log = new Logic.GameConsoleLog(logText, System.DateTime.Now, color, Logic.GameConsoleLog.LogType.game, true);
            GameConsoleController.Log(log);
        }

        private void AddLogEvent()
        {
            if (GameConsoleController.OnLog == null)
            {
                GameConsoleController.OnLog = new UnityAction<Logic.GameConsoleLog>((Logic.GameConsoleLog log) => RefreshLogs());
                return;
            }
            GameConsoleController.OnLog += (Logic.GameConsoleLog log) => RefreshLogs();
        }

        private void Start() => RefreshLogs();
        public void AssignConfig() => GameConsoleController.AssignConfig(ConsoleConfig);

        private void OnDestroy()
        {
            GameConsoleController.OnLog -= (Logic.GameConsoleLog log) => RefreshLogs();
            Application.logMessageReceived -= (string log, string trace, LogType type) => HandleUnityLog(log, trace, type);
        }

        private void LogLoadedScene(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= LogLoadedScene;
            GameConsoleController.Log($"Loaded scene {scene.name}", "scene", Logic.GameConsoleLog.LogType.game);
        }

        private void ReloadInterface()
        {
            if (CanvasObject == null) return;
            CanvasObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
            CanvasObject.SetActive(false);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && CanvasObject != null && CanvasObject.activeSelf) RunCommand();
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && CanvasObject.activeSelf) ReInsertCommand(false);
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && CanvasObject.activeSelf) ReInsertCommand(true);
        }

        public void DiscardPreviousCommand() => _commandIndex = -1;

        private void ReInsertCommand(bool reverse)
        {
            if (GameConsoleController.InvokedCommands.Count == 0) return;
            switch (reverse)
            {
                case false:
                    _commandIndex++;
                    break;
                case true:
                    _commandIndex--;
                    break;
            }
            _commandIndex = Mathf.Clamp(_commandIndex, 0, GameConsoleController.InvokedCommands.Count - 1);
            if (Input != null) 
                Input.text = GameConsoleController.InvokedCommands[GameConsoleController.InvokedCommands.Count - _commandIndex - 1];
        }

        /// <summary>Updates logs from controller</summary>
        public void RefreshLogs()
        {
            if (Logs == null) return;
            bool isSnapped = false;
            if (Scroll != null) isSnapped = Scroll.verticalNormalizedPosition == 0;
            Logs.text = GameConsoleController.LogsToString(LogLimit);
            if (isSnapped) ResetScroll();
        }

        public void ToggleConsole()
        {
            ToggleConsole(!CanvasObject.activeSelf);
        }

        public void ToggleConsole(bool state)
        {
            if (state) StartCoroutine(Reselect());
            if (Input != null) Input.text = "";
            OnConsoleChangeState.Invoke(state);
            if(CanvasObject != null) CanvasObject.SetActive(state);
            DiscardPreviousCommand();
            ResetScroll();
        }

        public void RunCommand()
        {
            DiscardPreviousCommand();
            if (Input == null) return;
            StartCoroutine(Reselect());

            if (Input.text == "")
            {
                ResetScroll();
                return;
            }

            GameConsoleController.Log(Input.text, "default", Logic.GameConsoleLog.LogType.user);
            GameConsoleController.RunCommand(Input.text);
            Input.text = "";
            RefreshLogs();
            ResetScroll();
        }

        private IEnumerator Reselect()
        {
            if (System.Array.IndexOf(IgnoreReselectPlatforms, Application.platform) >= 0) yield break;
            yield return null;
            Input.ActivateInputField();
        }

        public void ResetScroll()
        {
            if (Scroll == null) return;
            Canvas.ForceUpdateCanvases();
            Scroll.verticalNormalizedPosition = 0f;
        }
    }
}