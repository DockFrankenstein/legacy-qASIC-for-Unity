using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using qASIC.Displayer;
using qASIC.Displayer.Displayers;
using qASIC.Toggling;
using qASIC.Console;

namespace qASIC.Tools
{
    public static class qASICObjectCreator
    {
        #region Menu Items
        [MenuItem("GameObject/qASIC/Game console", false, 1)]
        static void CreateConsole()
        {
            GameObject consoleObject = new GameObject("Game console");
            GameObject canvasObject = CreateCanvas(consoleObject.transform, "Canvas", 10).gameObject;
            CreateImageObject(canvasObject.transform, new Color(0f, 0f, 0f, 0.5f), "Fade");

            RectTransform body = CreateImageObject(canvasObject.transform, new Color(0f, 0f, 0f), "Body").rectTransform;
            body.offsetMin = new Vector2(100f, 100f);
            body.offsetMax = new Vector2(-100f, -100f);

            TextMeshProUGUI text = CreateTextObject(null, "Text", 40);
            text.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scroll = CreateScroll(body, text.rectTransform);
            RectTransform scrollTrans = scroll.GetComponent<RectTransform>();
            text.rectTransform.offsetMin = new Vector2(10f, 0f);
            text.rectTransform.offsetMax = new Vector2(-10f, 0f);
            scrollTrans.offsetMin = new Vector2(scrollTrans.offsetMin.x, 100f);

            TMP_InputField field = CreateInputField(body, Color.white);
            RectTransform fieldTrans = field.GetComponent<RectTransform>();
            SetAnchors(fieldTrans, new Vector2(0f, 0f), new Vector2(1f, 0f));

            fieldTrans.sizeDelta = new Vector2(fieldTrans.sizeDelta.x, 100f);
            fieldTrans.anchoredPosition = new Vector2(fieldTrans.anchoredPosition.x, 50);

            CreateToggler(consoleObject, "console", canvasObject, KeyCode.BackQuote);
            GameConsoleInterface consoleScript = CreateInterface(consoleObject, text, field, scroll);

            field.onValueChanged.AddListener((string text) => consoleScript.DiscardPreviousCommand());

            canvasObject.SetActive(false);

            FinishObject(consoleObject);
        }

        [MenuItem("GameObject/qASIC/Displayer", false, 1)]
        static void CreateDisplayer()
        {
            GameObject displayerObject = new GameObject("Info displayer");
            GameObject canvasObject = CreateCanvas(displayerObject.transform, "Canvas", 9, false).gameObject;

            RectTransform image = CreateImageObject(canvasObject.transform, new Color(0f, 0f, 0f, 0.5f)).rectTransform;
            SetAnchors(image, Vector2.zero, new Vector2(0.5f, 1f));

            TextMeshProUGUI text = CreateTextObject(image.transform, "Text", 48);
            text.alignment = TextAlignmentOptions.Top;
            text.color = Color.white;

            CreateToggler(displayerObject, "main displayer", canvasObject, KeyCode.F3);
            CreateDisplayer(displayerObject, text);
            displayerObject.AddComponent<BasicDisplayer>();
            displayerObject.AddComponent<VersionDisplayer>();

            canvasObject.SetActive(false);

            FinishObject(displayerObject);
        }

        [MenuItem("GameObject/qASIC/Options load", false, 2)]
        static void CreateOptionsLoad()
        {
            GameObject obj = new GameObject("Options load");
            obj.AddComponent<Options.OptionsLoad>();
            FinishObject(obj);
        }

        [MenuItem("GameObject/qASIC/Input assign", false, 2)]
        static void CreateInputAssign()
        {
            GameObject obj = new GameObject("Input assign");
            obj.AddComponent<InputManagement.SetGlobalInputKeys>();
            FinishObject(obj);
        }

        [MenuItem("GameObject/qASIC/Audio manager", false, 3)]
        static void CreateAudioManager()
        {
            GameObject obj = new GameObject("Audio manager");
            obj.AddComponent<AudioManagment.AudioManager>();
            FinishObject(obj);
        }
        #endregion

        #region UI Create
        static Canvas CreateCanvas(Transform parent = null, string name = "Canvas", int order = 0, bool useRaycaster = true)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = order;

            go.AddComponent<CanvasScaler>();
            if (useRaycaster) go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        static Image CreateImageObject(Transform parent, Color color, string name = "Image")
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            RectTransform trans = go.AddComponent<RectTransform>();
            Image image = go.AddComponent<Image>();
            image.color = color;
            SetAnchors(trans, Vector2.zero, Vector2.one);
            StretchToAnchors(trans);
            return image;
        }

        static TextMeshProUGUI CreateTextObject(Transform parent, string name = "Text", int fontSize = 40)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            RectTransform trans = go.AddComponent<RectTransform>();
            SetAnchors(trans, Vector2.zero, Vector2.one);
            StretchToAnchors(trans);
            TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
            text.color = Color.black;
            return text;
        }

        static TMP_InputField CreateInputField(Transform parent, Color color, string name = "Input field", int fontSize = 40)
        {
            Image image = CreateImageObject(parent, color, name);
            TMP_InputField field = image.gameObject.AddComponent<TMP_InputField>();

            GameObject mask = new GameObject("Text");
            RectTransform maskTrans = mask.AddComponent<RectTransform>();
            mask.transform.SetParent(field.transform);
            SetAnchors(maskTrans, Vector2.zero, Vector2.one);
            StretchToAnchors(maskTrans);
            mask.AddComponent<RectMask2D>();
            maskTrans.offsetMin = new Vector2(10f, 5f);
            maskTrans.offsetMax = new Vector2(-10f, -5f);

            TextMeshProUGUI placeholder = CreateTextObject(mask.transform);
            TextMeshProUGUI text = CreateTextObject(mask.transform);

            SetAnchors(placeholder.rectTransform, Vector2.zero, Vector2.one);
            StretchToAnchors(placeholder.rectTransform);
            SetAnchors(text.rectTransform, Vector2.zero, Vector2.one);
            StretchToAnchors(text.rectTransform);

            placeholder.text = "Enter text...";
            placeholder.fontStyle = FontStyles.Italic;
            placeholder.color = new Color(0f, 0f, 0f, 0.5f);
            placeholder.alignment = TextAlignmentOptions.Left;
            text.alignment = TextAlignmentOptions.Left;

            field.placeholder = placeholder;
            field.textComponent = text;

            return field;
        }
        
        static ScrollRect CreateScroll(Transform parent, RectTransform content, string name = "Scroll View")
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);
            RectTransform trans = obj.AddComponent<RectTransform>();

            ScrollRect scroll = obj.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30f;
            scroll.verticalScrollbarSpacing = 0f;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

            Scrollbar scrollbar = CreateScrollbar(scroll.transform, new Color(0.01f, 0.01f, 0.01f), Color.white);

            RectTransform viewPort = CreateImageObject(scroll.transform, Color.white, "Viewport").rectTransform;
            viewPort.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            viewPort.pivot = new Vector2(0f, 1f);
            scroll.viewport = viewPort;

            content.transform.SetParent(viewPort);
            scroll.content = content;
            content.pivot = new Vector2(0.5f, 1f);
            SetAnchors(content, new Vector2(0f, 1f), new Vector2(1f, 1f));

            RectTransform scrollbarTrans = scrollbar.GetComponent<RectTransform>();
            SetAnchors(scrollbarTrans, new Vector2(1f, 0f), new Vector2(1f, 1f));
            StretchToAnchors(scrollbarTrans);

            scrollbarTrans.offsetMax = new Vector2(20f, scrollbarTrans.offsetMax.y);
            scrollbarTrans.anchoredPosition = new Vector2(-10f, 0f);
            scroll.verticalScrollbar = scrollbar;

            SetAnchors(trans, Vector2.zero, Vector2.one);
            StretchToAnchors(trans);
            return scroll;
        }

        static Scrollbar CreateScrollbar(Transform parent, Color backColor, Color handleColor, string name = "Scrollbar")
        {
            RectTransform trans = CreateImageObject(parent, backColor, name).rectTransform;
            Scrollbar scroll = trans.gameObject.AddComponent<Scrollbar>();

            scroll.direction = Scrollbar.Direction.BottomToTop;

            RectTransform slidingArea = new GameObject("Sliding Area").AddComponent<RectTransform>();
            slidingArea.transform.SetParent(trans);
            SetAnchors(slidingArea, Vector2.zero, Vector2.one);
            StretchToAnchors(slidingArea);

            RectTransform handle = CreateImageObject(parent, handleColor, "Handle").rectTransform;
            handle.transform.SetParent(slidingArea);
            SetAnchors(handle, Vector2.zero, Vector2.one);
            StretchToAnchors(handle);

            scroll.handleRect = handle;

            return scroll;
        }
        #endregion

        #region qASIC Scripts
        static StaticTogglerBasic CreateToggler(GameObject target, string tag, GameObject toggleObject = null, KeyCode key = KeyCode.F2)
        {
            StaticTogglerBasic toggler = target.AddComponent<StaticTogglerBasic>();
            toggler.AddToDontDestroy = true;
            toggler.Tag = tag;
            toggler.Key = key;
            toggler.ToggleObject = toggleObject;

            return toggler;
        }

        static InfoDisplayer CreateDisplayer(GameObject target, TextMeshProUGUI text)
        {
            InfoDisplayer displayer = target.AddComponent<InfoDisplayer>();

            displayer.DefaultLines = new DisplayerLine[]
                {
                    new DisplayerLine("fps", "Framerate"),
                    new DisplayerLine("resolution", "Resolution"),
                    new DisplayerLine("fullscreen", "Fullscreen mode"),
                    new DisplayerLine("gpu", "Graphics card"),
                    new DisplayerLine("cpu", "Processor"),
                    new DisplayerLine("cpu threads", "Processor threads"),
                    new DisplayerLine("memory", "Memory"),
                    new DisplayerLine("os", "Operating system"),
                    new DisplayerLine("version", "Version"),
                    new DisplayerLine("unity version", "Unity version"),
                    new DisplayerLine("qasic version", "qASIC version"),
                };

            displayer.Text = text;
            displayer.ExceptUnknown = false;

            return displayer;
        }

        static GameConsoleInterface CreateInterface(GameObject target, TextMeshProUGUI text, TMP_InputField input, ScrollRect scroll)
        {
            GameConsoleInterface console = target.AddComponent<GameConsoleInterface>();

            console.Logs = text;
            console.Input = input;
            console.Scroll = scroll;

            return console;
        }
        #endregion

        #region Transform Manipulation
        static void SetAnchors(RectTransform trans, Vector2 min, Vector2 max)
        {
            trans.anchorMin = min;
            trans.anchorMax = max;
        }

        static void StretchToAnchors(RectTransform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.offsetMin = Vector2.zero;
            trans.offsetMax = Vector2.zero;
        }
        #endregion

        #region Other
        static bool CheckNull(object obj)
        { 
            if(obj == null)
            {
                Debug.LogError("Couldn't create GameObject! Object is null!");
                return true;
            }
            return false;
        }

        static void FinishObject(GameObject obj)
        {
            Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            Selection.activeGameObject = obj;
        }
        #endregion
    }
}
