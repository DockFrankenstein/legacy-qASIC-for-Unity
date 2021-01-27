using UnityEngine;
using TMPro;
using qASIC.Console.Tools;
using System.Collections;

namespace qASIC.Console
{
    public class GameConsoleInterface : MonoBehaviour
    {
        [Header("Settings")]
        public int logLimit = 64;
        public GameConsoleConfig consoleConfig;
        public bool selectOnOpen = true;
        public bool reselectOnSubmit = true;

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
        public void RefreshLogs() { if (logs != null) logs.text = GameConsoleController.LogToString(logLimit); }

        private void ToggleConsole(bool state)
        {
            if (state && selectOnOpen) StartCoroutine(Reselect());
            else if (input != null) input.text = "";
            onConsoleChangeState.Invoke(state);
            if(canvasObject != null) canvasObject.SetActive(state);
            RefreshLogs();
        }

        private void RunCommand()
        {
            if (input == null) return;
            if (reselectOnSubmit) StartCoroutine(Reselect());
            if (input.text == "") return;
            GameConsoleController.Log(input.text, "default", Logic.GameConsoleLog.LogType.user, false);
            GameConsoleController.RunCommand(input.text);
            input.text = "";
        }

        private IEnumerator Reselect()
        {
            yield return null;
            input.ActivateInputField();
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