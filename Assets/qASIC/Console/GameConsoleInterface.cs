using UnityEngine;
using TMPro;
using System.Collections;
using qASIC.Toggling;
using qASIC.Console.Internal;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace qASIC.Console
{
    public class GameConsoleInterface : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Limit of logs that will be displayed. By default it is set to 64, for professional use 512 or 1024 is recomended")]
        public int logLimit = 64;

        [Header("Objects")]
        [Tooltip("TextMesh Pro text that will display logs")]
        public TextMeshProUGUI logText;
        [Tooltip("The input field that will enter commands")]
        public TMP_InputField inputField;
        [Tooltip("The scrollRect that contains Logs")]
        public UnityEngine.UI.ScrollRect scrollRect;

        private int _commandIndex = -1;

        bool init = false;

        public bool IsScrollSnapped { get; private set; } = true;

        Toggler toggler;

        [Tooltip("Platforms on which the input field won't be reselected after running a command or enabling the console")]
        public RuntimePlatform[] ignoreReselectPlatforms = new RuntimePlatform[]
        {
            RuntimePlatform.IPhonePlayer,
            RuntimePlatform.Android,
        };

        [Tooltip("Console configuration asset that will be sent to the controller on awake")]
        public GameConsoleConfig ConsoleConfig;

        public virtual void Awake()
        {
            if (init) return;

            init = true;

            toggler = GetComponent<Toggler>();
            if (toggler != null)
                toggler.OnChangeState.AddListener(OnChangeState);

            GameConsoleController.AssignConfig(ConsoleConfig);
            GameConsoleController.OnLog += _ => RefreshLogs();
            RefreshLogs();
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

        private void OnDestroy()
        {
            GameConsoleController.OnLog -= (GameConsoleLog log) => RefreshLogs();
        }

        public virtual void Update()
        {
            if (scrollRect != null) 
                IsScrollSnapped = scrollRect.horizontalNormalizedPosition == 0;

            HandleInput();
        }

        public virtual void HandleInput()
        {
            if (!toggler.State) return;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current[Key.Enter].wasPressedThisFrame) RunCommand();
            if (Keyboard.current[Key.UpArrow].wasPressedThisFrame) ReInsertCommand(false);
            if (Keyboard.current[Key.DownArrow].wasPressedThisFrame) ReInsertCommand(true);
#else
            if (Input.GetKeyDown(KeyCode.Return)) RunCommand();
            if (Input.GetKeyDown(KeyCode.UpArrow)) ReInsertCommand(false);
            if (Input.GetKeyDown(KeyCode.DownArrow)) ReInsertCommand(true);
#endif
        }

        public void DiscardPreviousCommand() =>
            _commandIndex = -1;

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

            string cmd = inputField.text;

            if (string.IsNullOrWhiteSpace(cmd))
            {
                ResetScroll();
                return;
            }

            inputField.SetTextWithoutNotify(string.Empty);

            GameConsoleController.Log(cmd, "default", GameConsoleLog.LogType.User);
            GameConsoleController.RunCommand(cmd);
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