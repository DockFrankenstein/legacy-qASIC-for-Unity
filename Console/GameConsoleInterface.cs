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
        public bool ResetScrollOnLog = true;

        [Header("Objects")]
        public GameObject CanvasObject;
        public TextMeshProUGUI Logs;
        public TMP_InputField Input;
        public UnityEngine.UI.ScrollRect Scroll;

        [Header("Events")]
        public UnityEventBool OnConsoleChangeState;
        public UnityAction LogListener;

        private void Awake()
        {
            AssignConfig();
            if (GameConsoleController.TryGettingConfig(out GameConsoleConfig config) && config.LogScene) 
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += LogLoadedScene;
        }
        private void Start() => RefreshLogs();
        public void AssignConfig() => GameConsoleController.AssignConfig(ConsoleConfig);
        private void FixedUpdate() => RefreshLogs();

        public void LogLoadedScene(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) =>
            GameConsoleController.Log($"Loaded scene {scene.name}", "scene", Logic.GameConsoleLog.LogType.game);

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && CanvasObject != null && CanvasObject.activeSelf == true) RunCommand();
            if (UnityEngine.Input.GetKeyDown(KeyCode.BackQuote) && CanvasObject != null) ToggleConsole(!CanvasObject.activeSelf);
        }

        /// <summary>Updates logs from controller</summary>
        public void RefreshLogs()
        {
            if (Logs != null) Logs.text = GameConsoleController.LogsToString(LogLimit);
            ResetScroll();
        }

        private void ToggleConsole(bool state)
        {
            if (state && SelectOnOpen) StartCoroutine(Reselect());
            else if (Input != null) Input.text = "";
            OnConsoleChangeState.Invoke(state);
            if(CanvasObject != null) CanvasObject.SetActive(state);
        }

        private void RunCommand()
        {
            if (Input == null) return;
            if (ReselectOnSubmit) StartCoroutine(Reselect());
            if (Input.text == "") return;
            GameConsoleController.Log(Input.text, "default", Logic.GameConsoleLog.LogType.user);
            GameConsoleController.RunCommand(Input.text);
            Input.text = "";
        }

        private IEnumerator Reselect()
        {
            yield return null;
            Input.ActivateInputField();
        }

        public void ResetScroll()
        {
            if (!ResetScrollOnLog || Scroll == null) return;
            Canvas.ForceUpdateCanvases();
            Scroll.verticalNormalizedPosition = 0f;
        }
    }
}