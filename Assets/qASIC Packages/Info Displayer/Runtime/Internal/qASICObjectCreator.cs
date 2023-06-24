using qASIC.Displayer.Displayers;
using qASIC.ProjectSettings;
using qASIC.Toggling;
using UnityEditor;
using UnityEngine;
using TMPro;

using static qASIC.Internal.qASICObjectCreator;

namespace qASIC.Displayer.Internal
{
    public static partial class qASICObjectCreator
    {
        public static void CreateDebugDisplyer()
        {
            var settings = DisplayerProjectSettings.Instance;

            GameObject displayerObject = new GameObject(settings.debugObjectName);
            displayerObject.SetActive(false);
            var canvasObject = CreateCanvas(displayerObject.transform, "Canvas", settings.debugCanvasOrder, false).gameObject;

            var text = CreateTextObject(canvasObject.transform, "Text", settings.debugFontSize);
            text.alignment = TextAlignmentOptions.TopLeft;
            text.color = Color.white;
            text.margin = new Vector4(10f, 10f, 10f, 10f);

            var displayer = CreateDisplayer(displayerObject, text);
            displayer.displayerName = settings.debugDisplayerName;
            displayer.defaultLines = new DisplayerLine[0];
            displayer.acceptUnknown = true;

            var toggler = displayer.gameObject.AddComponent<StaticTogglerBasic>();
            toggler.togglerTag = settings.debugTogglerName;
            toggler.key = settings.DebugTogglerKey;
            toggler.toggleObject = canvasObject;
            displayerObject.SetActive(true);
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/qASIC/Displayer", false, 1)]
        static void CreateDisplayer()
        {
            GameObject displayerObject = new GameObject("Info displayer");
            var canvasObject = CreateCanvas(displayerObject.transform, "Canvas", 9, false).gameObject;

            var image = CreateImageObject(canvasObject.transform, new Color(0f, 0f, 0f, 0.5f)).rectTransform;
            SetAnchors(image, Vector2.zero, new Vector2(0.5f, 1f));

            var text = CreateTextObject(image.transform, "Text", 48);
            text.alignment = TextAlignmentOptions.Top;
            text.color = Color.white;

            CreateToggler(displayerObject, "main displayer", canvasObject).key =
#if ENABLE_INPUT_SYSTEM
                UnityEngine.InputSystem.Key.F3;
#else
                KeyCode.F3;
#endif

            CreateDisplayer(displayerObject, text);
            displayerObject.AddComponent<BasicDisplayer>();
            displayerObject.AddComponent<VersionDisplayer>();

            canvasObject.SetActive(false);

            FinishObject(displayerObject);
        }
#endif

        public static InfoDisplayer CreateDisplayer(GameObject target, TextMeshProUGUI text)
        {
            var displayer = target.AddComponent<InfoDisplayer>();

            displayer.text = text;
            displayer.acceptUnknown = false;

            return displayer;
        }
    }
}