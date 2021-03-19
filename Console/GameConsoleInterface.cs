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
        public bool SelectOnOpen = true;
        public bool ReselectOnSubmit = true;
        public bool ResetScrollOnRun = true;

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
            if (GameConsoleController.TryGettingConfig(out GameConsoleConfig config) && config.LogScene)
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += LogLoadedScene;
            AddLogEvent();
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

        private void OnDestroy() => GameConsoleController.OnLog -= (Logic.GameConsoleLog log) => RefreshLogs();

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
            if (UnityEngine.Input.GetKeyDown(KeyCode.BackQuote) && CanvasObject != null) ToggleConsole();
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
            if (Logs != null) Logs.text = GameConsoleController.LogsToString(LogLimit);
        }

        public void ToggleConsole()
        {
            ToggleConsole(!CanvasObject.activeSelf);
        }

        public void ToggleConsole(bool state)
        {
            if (state && SelectOnOpen) StartCoroutine(Reselect());
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
            if (ReselectOnSubmit) StartCoroutine(Reselect());

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
            if (!ResetScrollOnRun || Scroll == null) return;
            Canvas.ForceUpdateCanvases();
            Scroll.verticalNormalizedPosition = 0f;
        }
    }
}