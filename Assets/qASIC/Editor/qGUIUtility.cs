using UnityEngine;
using UnityEditor;

namespace qASIC.EditorTools
{
    public static class qGUIUtility
    {
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

        public static Color BorderColor => EditorGUIUtility.isProSkin ? new Color(0.1372549019607843f, 0.1372549019607843f, 0.1372549019607843f) : new Color(0.6f, 0.6f, 0.6f);
        public static Texture2D BorderTexture => GenerateColorTexture(BorderColor);

        /// <summary>Draws serialized objects properties</summary>
        /// <param name="obj">Object to draw</param>
        /// <param name="skipFirst">Skips the first property. Do this if you want to hide the script field</param>
        public static void DrawObjectsProperties(SerializedObject obj, bool skipFirst = true)
        {
            SerializedProperty property = obj.GetIterator();
            if (!property.NextVisible(true)) return;

            do
            {
                if (skipFirst)
                {
                    skipFirst = false;
                    continue;
                }

                EditorGUILayout.PropertyField(property, true);
            }
            while (property.NextVisible(false));
        }

        public static void DrawPropertiesToEnd(SerializedObject obj, string startProperty) =>
            DrawPropertiesInRangeIfSpecified(obj, true, startProperty, false, string.Empty);

        public static void DrawPropertiesFromStart(SerializedObject obj, string endProperty) =>
            DrawPropertiesInRangeIfSpecified(obj, false, string.Empty, true, endProperty);

        public static void DrawPropertiesInRange(SerializedObject obj, string startProperty, string endProperty) =>
            DrawPropertiesInRangeIfSpecified(obj, true, startProperty, true, endProperty);


        private static void DrawPropertiesInRangeIfSpecified(SerializedObject obj, bool useStartProperty, string startProperty, bool useEndProperty, string endProperty)
        {
            SerializedProperty property = obj.GetIterator();
            if (!property.NextVisible(true)) return;

            bool draw = !useStartProperty;

            do
            {
                if (property.name == startProperty)
                    draw = true;

                if (draw)
                    EditorGUILayout.PropertyField(property, true);

                if (useEndProperty && property.name == endProperty)
                    return;
            }
            while (property.NextVisible(false));
        }

        /// <summary>Draws object like it's being viewed in the inspector</summary>
        /// <param name="obj">Object to draw</param>
        public static void DrawObjectsInspector(Object obj)
        {
            Editor editor = Editor.CreateEditor(obj);
            editor.OnInspectorGUI();
        }
    }
}