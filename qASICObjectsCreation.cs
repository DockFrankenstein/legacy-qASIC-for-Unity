using UnityEngine;
using UnityEngine.UI;
using TMPro;
using qASIC.InputManagement.Menu;

namespace qASIC.Backend
{
    public static class qASICObjectsCreation
    {
        public static GameObject CreateInputWindow(string newKeyName)
        {
            GameObject canvas = CreateCanvas();

            InputAssign assign = canvas.AddComponent<InputAssign>();
            assign.KeyName = newKeyName;

            InputListener listener = canvas.AddComponent<InputListener>();
            listener.StartListening(true, true);
            listener.onInputRecived.AddListener(assign.Assign);

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
            backImage.color = new Color(0f, 0f, 0f);

            //text
            GameObject text = new GameObject("Text");
            text.transform.parent = backColor.transform;

            TextMeshProUGUI textText = text.AddComponent<TextMeshProUGUI>();
            textText.text = "Assign the new key";
            textText.color = new Color(1f, 1f, 1f);

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

            newCanvas.AddComponent<GraphicRaycaster>();

            Canvas.ForceUpdateCanvases();

            return newCanvas;
        }
    }
}