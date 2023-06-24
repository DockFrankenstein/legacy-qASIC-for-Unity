using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using qASIC.Toggling;

namespace qASIC.Internal
{
    public static partial class qASICObjectCreator
    {
        #region UI Create
        public static Canvas CreateCanvas(Transform parent = null, string name = "Canvas", int order = 0, bool useRaycaster = true)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = order;

            go.AddComponent<CanvasScaler>();
            if (useRaycaster) go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static Image CreateImageObject(Transform parent, Color color, string name = "Image")
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            var trans = go.AddComponent<RectTransform>();
            var image = go.AddComponent<Image>();
            image.color = color;
            SetAnchors(trans, Vector2.zero, Vector2.one);
            StretchToAnchors(trans);
            return image;
        }

        public static TextMeshProUGUI CreateTextObject(Transform parent, string name = "Text", int fontSize = 40)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            var trans = go.AddComponent<RectTransform>();
            var text = go.AddComponent<TextMeshProUGUI>();
            text.color = Color.black;
            text.fontSize = fontSize;
            SetAnchors(trans, Vector2.zero, Vector2.one);
            StretchToAnchors(trans);
            return text;
        }

        public static TMP_InputField CreateInputField(Transform parent, Color color, string name = "Input field", int fontSize = 40)
        {
            var image = CreateImageObject(parent, color, name);
            var field = image.gameObject.AddComponent<TMP_InputField>();

            GameObject mask = new GameObject("Text");
            var maskTrans = mask.AddComponent<RectTransform>();
            mask.transform.SetParent(field.transform);
            SetAnchors(maskTrans, Vector2.zero, Vector2.one);
            StretchToAnchors(maskTrans);
            mask.AddComponent<RectMask2D>();
            maskTrans.offsetMin = new Vector2(10f, 5f);
            maskTrans.offsetMax = new Vector2(-10f, -5f);

            var placeholder = CreateTextObject(mask.transform);
            var text = CreateTextObject(mask.transform);

            SetAnchors(placeholder.rectTransform, Vector2.zero, Vector2.one);
            StretchToAnchors(placeholder.rectTransform);
            SetAnchors(text.rectTransform, Vector2.zero, Vector2.one);
            StretchToAnchors(text.rectTransform);

            placeholder.text = "Enter text...";
            placeholder.fontStyle = FontStyles.Italic;
            placeholder.color = new Color(0f, 0f, 0f, 0.5f);
            placeholder.alignment = TextAlignmentOptions.Left;
            text.alignment = TextAlignmentOptions.Left;
            text.richText = false;

            field.placeholder = placeholder;
            field.textComponent = text;
            field.textViewport = maskTrans;

            return field;
        }

        public static ScrollRect CreateScroll(Transform parent, RectTransform content, string name = "Scroll View")
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);
            var trans = obj.AddComponent<RectTransform>();

            var scroll = obj.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30f;
            scroll.verticalScrollbarSpacing = 0f;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

            var scrollbar = CreateScrollbar(scroll.transform, new Color(0.01f, 0.01f, 0.01f), Color.white);

            var viewPort = CreateImageObject(scroll.transform, Color.white, "Viewport").rectTransform;
            viewPort.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            viewPort.pivot = new Vector2(0f, 1f);
            scroll.viewport = viewPort;

            content.transform.SetParent(viewPort);
            scroll.content = content;
            content.pivot = new Vector2(0.5f, 1f);
            SetAnchors(content, new Vector2(0f, 1f), new Vector2(1f, 1f));

            var scrollbarTrans = scrollbar.GetComponent<RectTransform>();
            SetAnchors(scrollbarTrans, new Vector2(1f, 0f), new Vector2(1f, 1f));
            StretchToAnchors(scrollbarTrans);

            scrollbarTrans.offsetMax = new Vector2(20f, scrollbarTrans.offsetMax.y);
            scrollbarTrans.anchoredPosition = new Vector2(-10f, 0f);
            scroll.verticalScrollbar = scrollbar;

            SetAnchors(trans, Vector2.zero, Vector2.one);
            StretchToAnchors(trans);
            return scroll;
        }

        public static Scrollbar CreateScrollbar(Transform parent, Color backColor, Color handleColor, string name = "Scrollbar")
        {
            //scrollbar
            var trans = CreateImageObject(parent, backColor, name).rectTransform;
            var scroll = trans.gameObject.AddComponent<Scrollbar>();

            scroll.direction = Scrollbar.Direction.BottomToTop;

            //sliding area
            var slidingArea = new GameObject("Sliding Area").AddComponent<RectTransform>();
            slidingArea.transform.SetParent(trans);
            SetAnchors(slidingArea, Vector2.zero, Vector2.one);
            StretchToAnchors(slidingArea);

            //handle
            var handle = CreateImageObject(parent, handleColor, "Handle");
            handle.transform.SetParent(slidingArea);
            SetAnchors(handle.rectTransform, Vector2.zero, Vector2.one);
            StretchToAnchors(handle.rectTransform);

            //setting handle for scrollbar
            scroll.handleRect = handle.rectTransform;
            scroll.targetGraphic = handle;

            return scroll;
        }
        #endregion

        #region qASIC Scripts
        public static StaticTogglerBasic CreateToggler(GameObject target, string tag, GameObject toggleObject = null)
        {
            var toggler = target.AddComponent<StaticTogglerBasic>();
            toggler.addToDontDestroy = true;
            toggler.togglerTag = tag;
            toggler.toggleObject = toggleObject;

            return toggler;
        }
        #endregion

        #region Transform Manipulation
        public static void SetAnchors(RectTransform trans, Vector2 min, Vector2 max)
        {
            trans.anchorMin = min;
            trans.anchorMax = max;
        }

        public static void StretchToAnchors(RectTransform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.offsetMin = Vector2.zero;
            trans.offsetMax = Vector2.zero;
        }
        #endregion

        #region Other
        public static void FinishObject(GameObject obj)
        {
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            Selection.activeGameObject = obj;
#endif
        }

        public static void CheckForEventSystem()
        {
#if UNITY_EDITOR
            if (Resources.FindObjectsOfTypeAll<UnityEngine.EventSystems.EventSystem>().Length != 0) return;
            EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");
#endif
        }
        #endregion
    }
}