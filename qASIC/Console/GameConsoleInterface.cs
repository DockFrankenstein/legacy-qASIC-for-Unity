using UnityEngine;
using TMPro;
using qASIC.Console.Tools;
using System.Collections;
using UnityEngine.Events;

namespace qASIC.Console
{
    [RequireComponent(typeof(Toggler))]
    public class GameConsoleInterface : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Limit of logs that will be displayed. By default it is set to 64, for professional use 512 or 1024 is recomended")]
        public int LogLimit = 64;
        [Tooltip("Console configuration asset that will be sent to the controller on awake")]
        public GameConsoleConfig ConsoleConfig;

        [Header("Objects")]
        [Tooltip("TextMesh Pro text that will display logs")]
        public TextMeshProUGUI Logs;
        [Tooltip("The input field that will enter commands")]
        public TMP_InputField Input;
        [Tooltip("The scrollRect that contains Logs")]
        public UnityEngine.UI.ScrollRect Scroll;

        private int _commandIndex = -1;

        public bool isScrollSnapped { get; private set; } = true;

        Toggler toggler;

        [Tooltip("Platforms on which the input field won't be reselected after running a command or enabling the console")]
        public RuntimePlatform[] IgnoreReselectPlatforms = new RuntimePlatform[]
            {
                RuntimePlatform.IPhonePlayer,
                RuntimePlatform.Android,
            };

        private void Awake()
        {
            AssignConfig();
            SetupConfig();
            AddLogEvent();
            RefreshLogs();

            toggler = GetComponent<Toggler>();
            if (toggler != null) toggler.OnChangeState.AddListener(OnChangeState);
        }

        public void OnChangeState(bool state)
        {
            if (!state) return;
            ResetScroll();
            RefreshLogs();
            DiscardPreviousCommand();
            Input?.SetTextWithoutNotify(string.Empty);
            StartCoroutine(Reselect());
        }

        /// <summary>Enables features like logging messages to console, or logging the scene</summary>
        private void SetupConfig()
        {
            if (ConsoleConfig == null) return;
            if (ConsoleConfig.LogScene) UnityEngine.SceneManagement.SceneManager.sceneLoaded += LogLoadedScene;
            Application.logMessageReceived += HandleUnityLog;
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

        private void Update()
        {
            if (Scroll != null) isScrollSnapped = Scroll.horizontalNormalizedPosition == 0;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && toggler.state) RunCommand();
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && toggler.state) ReInsertCommand(false);
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && toggler.state) ReInsertCommand(true);
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
                Input.SetTextWithoutNotify(GameConsoleController.InvokedCommands[GameConsoleController.InvokedCommands.Count - _commandIndex - 1]);
        }

        /// <summary>Updates logs from controller</summary>
        public void RefreshLogs()
        {
            if (Logs == null) return;
            Logs.text = GameConsoleController.LogsToString(LogLimit);
            if (isScrollSnapped) ResetScroll();
        }

        public void RunCommand()
        {
            DiscardPreviousCommand();
            StartCoroutine(Reselect());
            if (Input == null) return;

            if (string.IsNullOrWhiteSpace(Input.text))
            {
                ResetScroll();
                return;
            }

            GameConsoleController.Log(Input.text, "default", Logic.GameConsoleLog.LogType.user);
            GameConsoleController.RunCommand(Input.text);
            Input.SetTextWithoutNotify(string.Empty);
            RefreshLogs();
            ResetScroll();
        }

        private IEnumerator Reselect()
        {
            if (Input == null) yield break;
            if (System.Array.IndexOf(IgnoreReselectPlatforms, Application.platform) >= 0) yield break;
            yield return null;
            Input.ActivateInputField();
        }

        public void ResetScroll()
        {
            if (Scroll == null) return;
            Canvas.ForceUpdateCanvases();
            Scroll.normalizedPosition = new Vector2(Scroll.normalizedPosition.x, 0f); ;
        }
    }
}