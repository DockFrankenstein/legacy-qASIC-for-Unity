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
                DrawItemReference("Positive", map, positiveGUID, a => onValueChanged?.Invoke(a, negativeGUID));
                DrawItemReference("Negative", map, negativeGUID, a => onValueChanged?.Invoke(positiveGUID, a));
            }
        }

        public static void DrawItemReference(string label, InputMap map, string guid, Action<string> onChangeValue)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(label);

                string itemName = map.ItemsDictionary.TryGetValue(guid, out var item) ?
                    item.ItemName :
                    "None";

                if (GUILayout.Button(itemName, EditorStyles.popup))
                    InputItemReferenceExplorer.OpenSelectWindow(guid, onChangeValue);

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
    }
}
#endif