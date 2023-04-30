#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

using static qASIC.qGUIUtility;
using UnityObject = UnityEngine.Object;

namespace qASIC.EditorTools
{
    public static class qGUIEditorUtility
    {
        //Editor icons
        public static Texture ErrorIcon => EditorGUIUtility.IconContent("console.erroricon").image;
        public static Texture WarningIcon => EditorGUIUtility.IconContent("console.warnicon").image;
        public static Texture InfoIcon => EditorGUIUtility.IconContent("console.infoicon").image;
        public static Texture PlusIcon => EditorGUIUtility.IconContent("Toolbar Plus").image;
        public static Texture PlusIconMore => EditorGUIUtility.IconContent("Toolbar Plus More").image;
        public static Texture MinusIcon => EditorGUIUtility.IconContent("Toolbar Minus").image;
        public static Texture MinusIconMore => EditorGUIUtility.IconContent("Toolbar Minus More").image;

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
        public static Color ButtonColor => EditorGUIUtility.isProSkin ?
                new Color(0.345098f, 0.345098f, 0.345098f) :
                new Color(0.8941177f, 0.8941177f, 0.8941177f);
        public static Texture2D ButtonColorTexture => GenerateColorTexture(ButtonColor);
        public static Color DarkBackgroundColor => EditorGUIUtility.isProSkin ?
                new Color(0.2078432f, 0.2078432f, 0.2078432f) :
                new Color(0.7137255f, 0.7137255f, 0.7137255f);
        public static Texture2D DarkBackgroundTexture => GenerateColorTexture(DarkBackgroundColor);

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

        public static Rect DrawProperty(Rect rect, SerializedProperty property)
        {
            property = property.Copy();
            int startDepth = property.depth;
            float height = 0f;

            Rect r = rect.SetHeight(EditorGUIUtility.singleLineHeight);

            if (property.NextVisible(true) && property.depth > startDepth)
            {
                DrawCurrent();
                while (property.NextVisible(false) && property.depth > startDepth)
                    DrawCurrent();
            }

            return rect.SetHeight(height);

            void DrawCurrent()
            {
                var p = property.Copy();
                EditorGUI.PropertyField(r, p);

                height += EditorGUI.GetPropertyHeight(p);
                r = r.MoveY(EditorGUI.GetPropertyHeight(p) + EditorGUIUtility.standardVerticalSpacing);
            }
        }

        public static void DrawPropertyLayout(SerializedProperty property)
        {
            property = property.Copy();

            int startDepth = property.depth;

            if (property.NextVisible(true) && property.depth > startDepth)
            {
                EditorGUILayout.PropertyField(property.Copy());
                while (property.NextVisible(false) && property.depth > startDepth)
                    EditorGUILayout.PropertyField(property.Copy());
            }
        }


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
        public static void DrawObjectsInspector(UnityObject obj)
        {
            Editor editor = Editor.CreateEditor(obj);
            editor.OnInspectorGUI();
        }

        public static void VerticalLineLayout()
        {
            GUIStyle style = new GUIStyle()
            {
                fixedWidth = 1f,
                stretchHeight = true,
            };

            style.normal.background = BorderTexture;
            GUILayout.Box(GUIContent.none, style);
        }

        public static void HorizontalLineLayout()
        {
            GUIStyle style = new GUIStyle()
            {
                fixedHeight = 1f,
                stretchWidth = true,
            };

            style.normal.background = BorderTexture;
            GUILayout.Box(GUIContent.none, style);
        }

        public static void VerticalLine(Rect rect)
        {
            GUIStyle style = new GUIStyle()
            {
                fixedWidth = 1f,
                stretchHeight = true,
            };

            style.normal.background = BorderTexture;

            if (Event.current.type == EventType.Repaint)
                style.Draw(rect, GUIContent.none, false, false, false, false);
        }

        public static void HorizontalLine(Rect rect)
        {
            GUIStyle style = new GUIStyle()
            {
                fixedHeight = 1f,
                stretchWidth = true,
            };

            style.normal.background = BorderTexture;

            if (Event.current.type == EventType.Repaint)
                style.Draw(rect, GUIContent.none, false, false, false, false);
        }

        public static void BorderAround(Rect rect)
        {
            GUIStyle horizontalLine = new GUIStyle()
            {
                fixedHeight = 1f,
            }
            .WithBackground(BorderTexture);

            GUIStyle verticalLine = new GUIStyle()
            {
                fixedWidth = 1f,
            }
            .WithBackground(BorderTexture);

            if (Event.current.type == EventType.Repaint)
            {
                verticalLine.Draw(rect.ResizeToLeft(0f), GUIContent.none, false, false, false, false);
                verticalLine.Draw(rect.ResizeToRight(0f), GUIContent.none, false, false, false, false);
                horizontalLine.Draw(rect.ResizeToTop(0f), GUIContent.none, false, false, false, false);
                horizontalLine.Draw(rect.ResizeToBottom(0f), GUIContent.none, false, false, false, false);
            }
        }

        /// <summary>Sorts a list using the search string</summary>
        /// <param name="list">List to sort</param>
        /// <param name="search">Search bar value</param>
        /// <returns>The sorted list</returns>
        public static IEnumerable<string> SortSearchList(IEnumerable<string> list, string search) =>
            SortSearchList(list, x => x, search);

        /// <summary>Sorts a list using the search string</summary>
        /// <param name="list">List to sort</param>
        /// <param name="func">Select the string</param>
        /// <param name="search">Search bar value</param>
        /// <returns>The sorted list</returns>
        public static IEnumerable<T> SortSearchList<T>(IEnumerable<T> list, Func<T, string> func, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                    return list;

            string[] keywords = search
                .ToLower()
                .Split(' ')
                .Where(s => s != string.Empty)
                .ToArray();

            return list
                .Select(x => new KeyValuePair<T, string>(x, func(x)))
                .GroupBy(x =>
                {
                    string s = x.Value.ToLower();
                    float percentage = 0f;
                    foreach (var keyword in keywords)
                    {
                        if (!s.Contains(keyword))
                            return 0f;

                        percentage += (float)s.Length / keyword.Length;
                        s = s.Replace(keyword, " ");
                    }

                    return percentage;
                })
                .Where(x => x.Key > 0f)
                .OrderBy(x => x.Key)
                .SelectMany(x => x)
                .Select(x => x.Key)
                .ToList();
        }
    }
}
#endif