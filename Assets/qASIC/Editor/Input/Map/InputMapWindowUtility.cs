#if UNITY_EDITOR
using qASIC.Tools;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using qASIC.EditorTools.Internal;
using UnityEditor;
using UnityEngine;
using qASIC.Input.Internal.ReferenceExplorers;
using qASIC.EditorTools;
using UnityEngine.UIElements;

namespace qASIC.Input.Map.Internal
{
    public static class InputMapWindowUtility
    {
        public static string GenerateUniqueName(string name, Func<string, bool> condition)
        {
            name = GetIndex(name, out int index);

            for (; condition.Invoke(GetUniqueIndexName(name, index)); index++) { }
            return GetUniqueIndexName(name, index);
        }

        private static string GetUniqueIndexName(string baseText, int index) =>
            index == 0 ? baseText : $"{baseText}{index - 1}";

        private static string GetIndex(string name, out int index)
        {
            index = 0;
            //Checks if there is a number at the end
            var m = Regex.Match(name, @"\d+$");
            if (!m.Success) return $"{name} ";

            //new index
            index = int.Parse(m.Value) + 1;
            //Removing the number
            name = name.Substring(0, name.LastIndexOf(m.Value));

            return name;
        }

        public static Type[] GetOtherItemTypes()
        {
            Type[] excludedTypes = new Type[]
            {
                typeof(InputBinding),
                typeof(InputMapItem),
                typeof(InputMapItem<>),
            };

            var types = TypeFinder.FindAllTypesList<InputMapItem>();

            foreach (var type in excludedTypes)
                types.Remove(type);

            return types
                .ToArray();
        }

        public static ItemType[] GetOtherItemTypesWithNames()
        {
            string[] ignorableNameStarts = new string[]
            {
                "inputmapitem",
                "inputmap",
                "inputitem",
                "input",
                "mapitem",
                "map",
            };

            var itemTypes = GetOtherItemTypes();

            var items = itemTypes
                .Select(x =>
                {
                    string name = x.Name
                        .Split('.')
                        .Last();

                    foreach (var start in ignorableNameStarts)
                    {
                        if (!name.ToLower().StartsWith(start)) continue;
                        name = name.Remove(0, start.Length);
                        name = Regex.Replace(name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
                        break;
                    }

                    return new ItemType(name, x);
                })
                .ToArray();

            return items;
        }

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
                DrawItemReference<InputBinding>("Positive", map, positiveGUID, a => onValueChanged?.Invoke(a, negativeGUID));
                DrawItemReference<InputBinding>("Negative", map, negativeGUID, a => onValueChanged?.Invoke(positiveGUID, a));
            }
        }

        public static void DrawItemReference(string label, InputMap map, string guid, Action<string> onChangeValue) =>
            DrawItemReference(label, map, guid, onChangeValue, typeof(InputMapItem));

        public static void DrawItemReference<T>(string label, InputMap map, string guid, Action<string> onChangeValue) =>
            DrawItemReference(label, map, guid, onChangeValue, typeof(T));

        public static void DrawItemReference(string label, InputMap map, string guid, Action<string> onChangeValue, Type type)
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

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(label);

                string itemName = map.ItemsDictionary.TryGetValue(guid, out var item) ?
                    $"{item.ItemName} ({map.groups.Where(x => x.items.Contains(item)).FirstOrDefault()})" :
                    "None";

                GUILayout.FlexibleSpace();

                Rect buttonRect = GUILayoutUtility.GetLastRect()
                    .SetHeight(18f)
                    .MoveY(2f)
                    .BorderLeft(4f);

                buttonStyle.normal.background = qGUIUtility.GenerateColorTexture(EditorGUIUtility.isProSkin ?
                    new Color(0.345098f, 0.345098f, 0.345098f) :
                    new Color(0.8941177f, 0.8941177f, 0.8941177f));
                    //buttonRect.Contains(Event.current.mousePosition) ?
                    //qGUIUtility.GenerateColorTexture(new Color(0.454902f, 0.454902f, 0.454902f)) :
                    //qGUIUtility.GenerateColorTexture(new Color(0.345098f, 0.345098f, 0.345098f));

                if (GUI.Button(buttonRect, itemName, buttonStyle))
                    InputItemReferenceExplorer.OpenSelectWindow(map, guid, onChangeValue, type);

                if (Event.current.type == EventType.Repaint)
                {
                    verticalLine.Draw(buttonRect.ResizeToLeft(0f), GUIContent.none, false, false, false, false);
                    verticalLine.Draw(buttonRect.ResizeToRight(0f), GUIContent.none, false, false, false, false);
                    horizontalLine.Draw(buttonRect.ResizeToTop(0f), GUIContent.none, false, false, false, false);
                    horizontalLine.Draw(buttonRect.ResizeToBottom(0f), GUIContent.none, false, false, false, false);
                }
            }
        }

        public struct ItemType
        {
            public ItemType(string name, Type type)
            {
                this.name = name;
                this.type = type;
            }

            public string name;
            public Type type;
        }

        static class Styles
        {
            public static GUIStyle ItemReferenceButton => new GUIStyle(EditorStyles.popup)
            {
                wordWrap = false,
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
            };
        }
    }
}
#endif