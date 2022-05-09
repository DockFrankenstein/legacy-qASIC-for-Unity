#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using static qASIC.qGUIUtility;

namespace qASIC.EditorTools
{
    public static class qGUIEditorUtility
    {
        //Editor icons
        public static Texture ErrorIcon => EditorGUIUtility.IconContent("console.erroricon").image;
        public static Texture WarningIcon => EditorGUIUtility.IconContent("console.warnicon").image;
        public static Texture InfoIcon => EditorGUIUtility.IconContent("console.infoicon").image;
        public static Texture PlusIcon => EditorGUIUtility.IconContent("Toolbar Plus").image;
        public static Texture MinusIcon => EditorGUIUtility.IconContent("Toolbar Minus").image;

        public static Texture ConvertIconTypeToTexture(InspectorMessageIconType icon)
        {
            switch (icon)
            {
                case InspectorMessageIconType.notification:
                    return InfoIcon;
                case InspectorMessageIconType.warning:
                    return WarningIcon;
                case InspectorMessageIconType.error:
                    return ErrorIcon;
                default:
                    return null;
            }
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

            //script reference
            bool isScript = !useStartProperty;

            do
            {
                if (property.name == startProperty)
                    draw = true;

                if (draw)
                {
                    var disabledScope = new EditorGUI.DisabledGroupScope(isScript);
                    using (disabledScope)
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                }

                isScript = false;

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
#endif