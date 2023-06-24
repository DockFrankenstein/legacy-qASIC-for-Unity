using qASIC.Console;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using static qASIC.Internal.qASICObjectCreator;

namespace qASIC.Input.Internal
{
    public static partial class qASICObjectCreator
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/qASIC/Game console", false, 1)]
        static void CreateConsole()
        {
            GameObject consoleObject = new GameObject("Game console");
            var canvasObject = CreateCanvas(consoleObject.transform, "Canvas", 10).gameObject;
            CreateImageObject(canvasObject.transform, new Color(0f, 0f, 0f, 0.5f), "Fade");

            var body = CreateImageObject(canvasObject.transform, new Color(0f, 0f, 0f), "Body").rectTransform;
            body.offsetMin = new Vector2(100f, 100f);
            body.offsetMax = new Vector2(-100f, -100f);

            var text = CreateTextObject(null, "Text", 40);
            text.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = CreateScroll(body, text.rectTransform);
            var scrollTrans = scroll.GetComponent<RectTransform>();
            text.rectTransform.offsetMin = new Vector2(10f, 0f);
            text.rectTransform.offsetMax = new Vector2(-10f, 0f);
            scrollTrans.offsetMin = new Vector2(scrollTrans.offsetMin.x, 100f);

            var field = CreateInputField(body, Color.white);
            var fieldTrans = field.GetComponent<RectTransform>();
            SetAnchors(fieldTrans, new Vector2(0f, 0f), new Vector2(1f, 0f));

            fieldTrans.sizeDelta = new Vector2(fieldTrans.sizeDelta.x, 100f);
            fieldTrans.anchoredPosition = new Vector2(fieldTrans.anchoredPosition.x, 50);

            CreateToggler(consoleObject, "console", canvasObject).key =
#if ENABLE_INPUT_SYSTEM
                UnityEngine.InputSystem.Key.Backquote;
#else
                KeyCode.BackQuote;
#endif

            var consoleScript = CreateInterface(consoleObject, text, field, scroll);

            field.onValueChanged.AddListener(_ => consoleScript.DiscardPreviousCommand());

            canvasObject.SetActive(false);

            CheckForEventSystem();

            FinishObject(consoleObject);
        }
#endif

        public static GameConsoleInterface CreateInterface(GameObject target, TextMeshProUGUI text, TMP_InputField input, ScrollRect scroll)
        {
            var console = target.AddComponent<GameConsoleInterface>();

            console.logText = text;
            console.inputField = input;
            console.scrollRect = scroll;

            return console;
        }
    }
}