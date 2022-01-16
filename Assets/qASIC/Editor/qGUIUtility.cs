using UnityEngine;
using UnityEditor;

namespace qASIC.EditorTools
{
    public static class qGUIUtility
    {
        #region qASIC
        public static Texture2D qASICBackgroundTexture => GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f));
        public static Color qASICColor => new Color(0f, 0.7019607843137255f, 1f);
        public static Texture2D qASICColorTexture => GenerateColorTexture(qASICColor);

        public static Color BorderColor => EditorGUIUtility.isProSkin ? new Color(0.1372549019607843f, 0.1372549019607843f, 0.1372549019607843f) : new Color(0.6f, 0.6f, 0.6f);
        public static Texture2D BorderTexture => GenerateColorTexture(BorderColor);

        public static void DrawqASICBanner(string docs = "https://docs.qasictools.com")
        {
            switch (GUILayout.SelectionGrid(-1, new GUIContent[] { new GUIContent("Asset store"), new GUIContent("Docs"), new GUIContent("Support") }, 3))
            {
                case 0:
                    Application.OpenURL("https://qasictools.com/store");
                    break;
                case 1:
                    Application.OpenURL(docs);
                    break;
                case 2:
                    Application.OpenURL("https://qasictools.com/support");
                    break;
            }

            EditorGUILayout.Space();
        }
        #endregion

        //Editor icons
        public static Texture ErrorIcon => EditorGUIUtility.IconContent("console.erroricon").image;
        public static Texture WarningIcon => EditorGUIUtility.IconContent("console.warnicon").image;
        public static Texture InfoIcon => EditorGUIUtility.IconContent("console.infoicon").image;
        public static Texture PlusIcon => EditorGUIUtility.IconContent("Toolbar Plus").image;
        public static Texture MinusIcon => EditorGUIUtility.IconContent("Toolbar Minus").image;

        public static Texture2D GenerateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public static void DrawMessageBox(string message, InspectorMessageIconType icon) =>
            DrawMessageBox(message, MessageAttribute.ConvertIconTypeToTexture(icon));

        public static void DrawMessageBox(string message, Texture icon = null)
        {
            GUIStyle style = new GUIStyle("Label")
            {
                wordWrap = true,
                alignment = TextAnchor.UpperLeft,
            };


            GUILayout.BeginHorizontal(new GUIStyle() { normal = new GUIStyleState() { background = GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f)) }, padding = new RectOffset(4, 4, 4, 4) });

            if (icon)
            {
                icon = Tools.TextureUtility.Resize((Texture2D)icon, 40, 40, FilterMode.Bilinear);
                GUILayout.Box(icon, GUILayout.Width(40), GUILayout.Height(40));
            }

            GUILayout.Label(message, style);

            GUILayout.EndHorizontal();
        }

        public static KeyCode KeyCodePopup(KeyCode selectedKey) =>
            (KeyCode)EditorGUILayout.EnumPopup(selectedKey);

        public static KeyCode KeyCodePopup(KeyCode selectedKey, string label) =>
            (KeyCode)EditorGUILayout.EnumPopup(label, selectedKey);
    }
}