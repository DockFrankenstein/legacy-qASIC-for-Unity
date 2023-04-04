using qASIC.EditorTools;
using qASIC.Input.Internal.ReferenceExplorers;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace qASIC.Input.Map.Internal
{
    public static partial class InputGUIUtility
    {
        public static void DrawAxis(string label, InputMapWindow window, Axis axis)
        {
            DrawAxis(label, window.Map, axis.positiveGuid, axis.negativeGuid, (a, b) =>
            {
                axis.positiveGuid = a;
                axis.negativeGuid = b;
                window.SetMapDirty();
            });
        }

        public static void DrawAxis(string label, InputMap map, string positiveGUID, string negativeGUID, Action<string, string> onValueChanged)
        {
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.Space();
                GUILayout.Label(label, EditorStyles.whiteLargeLabel);
            }

            using (new EditorChangeChecker.ChangeCheckPause())
            {
                DrawItemReferenceLayout<InputBinding>("Positive", map, positiveGUID, a => onValueChanged?.Invoke(a, negativeGUID));
                DrawItemReferenceLayout<InputBinding>("Negative", map, negativeGUID, a => onValueChanged?.Invoke(positiveGUID, a));
            }
        }

        public static void DrawItemReference(string label, InputMap map, string guid, Action<string> onChangeValue) =>
            DrawItemReferenceLayout(label, map, guid, onChangeValue, typeof(InputMapItem));

        public static void DrawItemReferenceLayout<T>(string label, InputMap map, string guid, Action<string> onChangeValue) =>
            DrawItemReferenceLayout(label, map, guid, onChangeValue, typeof(T));

        public static void DrawItemReferenceLayout(string label, InputMap map, string guid, Action<string> onChangeValue, Type type)
        {
            using (new GUILayout.VerticalScope(GUILayout.Height(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)))
                GUILayout.FlexibleSpace();

            Rect rect = GUILayoutUtility.GetLastRect();

            DrawItemReference(rect, new GUIContent(label), map, guid, onChangeValue, type);
        }

        public static void DrawItemReference(Rect rect, GUIContent label, InputMap map, string guid, Action<string> onChangeValue) =>
            DrawItemReference(rect, label, map, guid, onChangeValue, typeof(InputMapItem));

        public static void DrawItemReference<T>(Rect rect, GUIContent label, InputMap map, string guid, Action<string> onChangeValue) =>
            DrawItemReference(rect, label, map, guid, onChangeValue, typeof(T));

        public static void DrawItemReference(Rect rect, GUIContent label, InputMap map, string guid, Action<string> onChangeValue, Type type)
        {
            GUIStyle buttonStyle = new GUIStyle()
            {
                wordWrap = false,
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
                padding = new RectOffset(4, 4, 0, 0),
                normal = new GUIStyleState()
                {
                    textColor = EditorStyles.miniButton.normal.textColor,
                }
            };

            GUIStyle horizontalLine = new GUIStyle()
            {
                fixedHeight = 1f,
            }
            .WithBackground(qGUIEditorUtility.BorderTexture);

            GUIStyle verticalLine = new GUIStyle()
            {
                fixedWidth = 1f,
            }
            .WithBackground(qGUIEditorUtility.BorderTexture);

            rect = rect
                .SetHeight(18f);

            rect = EditorGUI.PrefixLabel(rect, label);

            string itemName = map.ItemsDictionary.TryGetValue(guid, out var item) ?
                $"{item.ItemName} ({map.groups.Where(x => x.items.Contains(item)).FirstOrDefault()})" :
                "None";

            buttonStyle.normal.background = qGUIUtility.GenerateColorTexture(EditorGUIUtility.isProSkin ?
                new Color(0.345098f, 0.345098f, 0.345098f) :
                new Color(0.8941177f, 0.8941177f, 0.8941177f));

            if (GUI.Button(rect, itemName, buttonStyle))
                InputItemReferenceExplorer.OpenSelectWindow(map, guid, onChangeValue, type);

            if (Event.current.type == EventType.Repaint)
            {
                verticalLine.Draw(rect.ResizeToLeft(0f), GUIContent.none, false, false, false, false);
                verticalLine.Draw(rect.ResizeToRight(0f), GUIContent.none, false, false, false, false);
                horizontalLine.Draw(rect.ResizeToTop(0f), GUIContent.none, false, false, false, false);
                horizontalLine.Draw(rect.ResizeToBottom(0f), GUIContent.none, false, false, false, false);
            }
        }
    }
}
