using UnityEngine;
using TMPro;

namespace qASIC.Console
{
    public class GameConsoleInterface : MonoBehaviour
    {
        public GameObject canvasObject;

        public TextMeshProUGUI logs;
        public TMP_InputField input;

        private void Update()
        {
            logs.text = GameConsoleController.logs;
            if (Input.GetKeyDown(KeyCode.Return) && canvasObject != null && canvasObject.activeSelf == true)
                RunCommand();

            if (Input.GetKeyDown(KeyCode.BackQuote) && canvasObject != null)
                canvasObject.SetActive(!canvasObject.activeSelf);
        }

        private void RunCommand()
        {
            if (input.text != "")
                GameConsoleController.RunCommand(input.text);
            input.text = "";
        }

        private void Awake()
        {
            if (GameConsoleController.logs == "")
            {
                GameConsoleController.LoadConfig();
                GameConsoleController.Log("Thank you for using qASIC console", "qASIC");
            }
        }
    }
}