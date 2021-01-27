using UnityEngine;
using TMPro;
using qASIC.Console.Tools;

namespace qASIC.Console
{
    public class GameConsoleInterface : MonoBehaviour
    {
        [Header("Settings")]
        public int logLimit = 64;
        public GameConsoleConfig consoleConfig;

        [Header("Objects")]
        public GameObject canvasObject;
        public TextMeshProUGUI logs;
        public TMP_InputField input;

        [Header("Events")]
        public UnityEventBool onConsoleChangeState;

        private static bool init = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && canvasObject != null && canvasObject.activeSelf == true) RunCommand();
            if (Input.GetKeyDown(KeyCode.BackQuote) && canvasObject != null) ToggleConsole(!canvasObject.activeSelf);
        }

        private void FixedUpdate() { if (canvasObject.activeSelf) RefreshLogs(); }
        public void RefreshLogs() => logs.text = GameConsoleController.LogToString(logLimit);

        private void ToggleConsole(bool state)
        {
            onConsoleChangeState.Invoke(state);
            canvasObject.SetActive(state);
            RefreshLogs();
        }

        private void RunCommand()
        {
            if (input.text == "") return;
            GameConsoleController.Log(input.text, "default", Logic.GameConsoleLog.LogType.user, false);
            GameConsoleController.RunCommand(input.text);
            input.text = "";
        }

        private void Awake()
        {
            GameConsoleController.AssignConfig(consoleConfig);
            Initialize();
        }

        private void Initialize()
        {
            if (init) return;
            init = true;
            if (GameConsoleController.TryGettingConfig(out GameConsoleConfig config) && config.showThankYouMessage) 
                GameConsoleController.Log("Thank you for using qASIC console", "qASIC");
        }
    }
}