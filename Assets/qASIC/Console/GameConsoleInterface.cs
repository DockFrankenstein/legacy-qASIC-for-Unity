using UnityEngine;
using TMPro;
using qASIC.Console.Tools;
using System.Collections;
using UnityEngine.Events;
using qASIC.Toggling;

namespace qASIC.Console
{
    public class GameConsoleInterface : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Limit of logs that will be displayed. By default it is set to 64, for professional use 512 or 1024 is recomended")]
        public int logLimit = 64;
        [Tooltip("Console configuration asset that will be sent to the controller on awake")]
        public GameConsoleConfig ConsoleConfig;

        [Header("Objects")]
        [Tooltip("TextMesh Pro text that will display logs")]
        public TextMeshProUGUI logText;
        [Tooltip("The input field that will enter commands")]
        public TMP_InputField inputField;
        [Tooltip("The scrollRect that contains Logs")]
        public UnityEngine.UI.ScrollRect scrollRect;

        private int _commandIndex = -1;

        public bool IsScrollSnapped { get; private set; } = true;

        Toggler toggler;

        [Tooltip("Platforms on which the input field won't be reselected after running a command or enabling the console")]
        public RuntimePlatform[] ignoreReselectPlatforms = new RuntimePlatform[]
            {
                RuntimePlatform.IPhonePlayer,
                RuntimePlatform.Android,
            };

        public virtual void Awake()
        {
            AssignConfig();
            SetupConfig();
            AddLogEvent();
            RefreshLogs();

            toggler = GetComponent<Toggler>();
            if (toggler != null) toggler.OnChangeState.AddListener(OnChangeState);
        }

        void OnChangeState(bool state)
        {
            if (!state) return;
            ResetScroll();
            RefreshLogs();
            DiscardPreviousCommand();
            inputField?.SetTextWithoutNotify(string.Empty);
            StartCoroutine(Reselect());
        }

        /// <summary>Enables features like logging messages to console, or logging the scene</summary>
        public void SetupConfig()
        {
            if (ConsoleConfig == null) return;
            if (ConsoleConfig.logScene) UnityEngine.SceneManagement.SceneManager.sceneLoaded += LogLoadedScene;
            Application.logMessageReceived += HandleUnityLog;
        }

        private void HandleUnityLog(string logText, string trace, LogType type)
        {
            if (!GameConsoleController.TryGettingConfig(out GameConsoleConfig config)) return;

            string color = "default";
            switch (type)
            {
                case LogType.Exception:
                    if (!config.logUnityExceptionsToConsole) return;
                    color = "unity exception";
                    break;
                case LogType.Error:
                    if (!config.logUnityErrorsToConsole) return;
                    color = "unity error";
                    break;
                case LogType.Assert:
                    if (!config.logUnityAssertsToConsole) return;
                    color = "unity assert";
                    break;
                case LogType.Warning:
                    if (!config.logUnityWarningsToConsole) return;
                    color = "unity warning";
                    break;
                case LogType.Log:
                    if (!config.logUnityMessagesToConsole) return;
                    color = "unity message";
                    break;
            }

            Logic.GameConsoleLog log = new Logic.GameConsoleLog(logText, System.DateTime.Now, color, Logic.GameConsoleLog.LogType.Game, true);
            GameConsoleController.Log(log);
        }

        private void AddLogEvent()
        {
            UnityAction<Logic.GameConsoleLog> refreshLogs = _ => RefreshLogs();
            if (GameConsoleController.OnLog == null)
            {
                GameConsoleController.OnLog = refreshLogs;
                return;
            }
            GameConsoleController.OnLog += refreshLogs;
        }

        public void AssignConfig() =>
            GameConsoleController.AssignConfig(ConsoleConfig);

        private void OnDestroy()
        {
            GameConsoleController.OnLog -= (Logic.GameConsoleLog log) => RefreshLogs();
            Application.logMessageReceived -= (string log, string trace, LogType type) => HandleUnityLog(log, trace, type);
        }

        public virtual void LogLoadedScene(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= LogLoadedScene;
            GameConsoleController.Log($"Loaded scene {scene.name}", "scene", Logic.GameConsoleLog.LogType.Game);
        }

        public virtual void Update()
        {
            if (scrollRect != null) 
                IsScrollSnapped = scrollRect.horizontalNormalizedPosition == 0;

            HandleInput();
        }

        public virtual void HandleInput()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && toggler.State) RunCommand();
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && toggler.State) ReInsertCommand(false);
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && toggler.State) ReInsertCommand(true);
        }

        public void DiscardPreviousCommand() => _commandIndex = -1;

        private void ReInsertCommand(bool reverse)
        {
            if (GameConsoleController.invokedCommands.Count == 0) return;
            _commandIndex += reverse ? -1 : 1;

            _commandIndex = Mathf.Clamp(_commandIndex, 0, GameConsoleController.invokedCommands.Count - 1);
            if (inputField != null)
                inputField.SetTextWithoutNotify(GameConsoleController.invokedCommands[GameConsoleController.invokedCommands.Count - _commandIndex - 1]);
        }

        /// <summary>Updates logs from controller</summary>
        public virtual void RefreshLogs()
        {
            if (logText == null) return;
            logText.text = GameConsoleController.LogsToString(logLimit);
            if (IsScrollSnapped) ResetScroll();
        }

        public virtual void RunCommand()
        {
            DiscardPreviousCommand();
            StartCoroutine(Reselect());
            if (inputField == null) return;

            if (string.IsNullOrWhiteSpace(inputField.text))
            {
                ResetScroll();
                return;
            }

            GameConsoleController.Log(inputField.text, "default", Logic.GameConsoleLog.LogType.User);
            GameConsoleController.RunCommand(inputField.text);
            inputField.SetTextWithoutNotify(string.Empty);
            RefreshLogs();
            ResetScroll();
        }

        IEnumerator Reselect()
        {
            if (inputField == null) yield break;
            if (System.Array.IndexOf(ignoreReselectPlatforms, Application.platform) >= 0) yield break;
            yield return null;
            inputField.ActivateInputField();
        }

        public void ResetScroll()
        {
            if (scrollRect == null) return;
            Canvas.ForceUpdateCanvases();
            scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 0f);
        }
    }
}