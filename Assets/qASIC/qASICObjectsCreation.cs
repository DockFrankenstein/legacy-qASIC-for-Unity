using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using qASIC.InputManagment;

namespace qASIC.Backend
{
    public static class qASICObjectsCreation
    {
        private static string lastKeyName = "";

        public static void test(KeyCode key)
        {
            InputManager.ChangeInput(lastKeyName, key, true);
        }

        public static GameObject CreateInputWindow(string newKeyName)
        {
            GameObject canvas = CreateCanvas();

            InputListiner listiner = canvas.AddComponent<InputListiner>();
            listiner.StartListening(true, true);
            lastKeyName = newKeyName;
            listiner.onInputRevived.AddListener(test);

            //back color
            GameObject backColor = new GameObject("Color");
            backColor.AddComponent<RectTransform>();
            backColor.transform.SetParent(canvas.transform, false);

            HorizontalLayoutGroup backColorLGroup = backColor.AddComponent<HorizontalLayoutGroup>();
            backColorLGroup.padding = new RectOffset(5, 5, 5, 5);
            backColorLGroup.childForceExpandHeight = false;
            backColorLGroup.childForceExpandWidth = false;
            backColorLGroup.childAlignment = TextAnchor.MiddleCenter;
            
            ContentSizeFitter backColorContentFit = backColor.AddComponent<ContentSizeFitter>();
            backColorContentFit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            backColorContentFit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Image backImage = backColor.AddComponent<Image>();
            backImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            //text
            GameObject text = new GameObject("Text");
            text.transform.parent = backColor.transform;

            TextMeshProUGUI textText = text.AddComponent<TextMeshProUGUI>();
            textText.text = "Assign the new key";

            ContentSizeFitter textContentFit = text.AddComponent<ContentSizeFitter>();
            textContentFit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            textContentFit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            text.transform.position = new Vector2(0f, 0f);

            return canvas;
        }

        private static GameObject CreateCanvas()
        {
            GameObject newCanvas = new GameObject();
            newCanvas.name = "Canvas";

            Canvas canvas = newCanvas.AddComponent<Canvas>();
            canvas.sortingOrder = 100;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler canvasScaler = newCanvas.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            /*GraphicRaycaster graphicsRaycaster = */newCanvas.AddComponent<GraphicRaycaster>();

            Canvas.ForceUpdateCanvases();

            return newCanvas;
        }
    }
}