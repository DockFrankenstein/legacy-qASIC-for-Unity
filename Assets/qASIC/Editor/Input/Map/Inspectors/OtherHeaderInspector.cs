using UnityEngine;
using UnityEditor;
using System;
using qASIC.Tools;
using System.Linq;
using System.Text.RegularExpressions;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class OtherHeaderInspector : InputMapItemInspector
    {
        CreatableItem[] _creatableItems;

        public override void Initialize(OnInitializeContext context)
        {
            base.Initialize(context);

            Type[] excludedTypes = new Type[]
            {
                typeof(InputBinding),
                typeof(InputMapItem),
                typeof(InputMapItem<>),
            };

            string[] ignorableNameStarts = new string[]
            {
                "inputmapitem",
                "inputmap",
                "inputitem",
                "input",
                "mapitem",
                "map",
            };

            var itemTypes = TypeFinder.FindAllTypesList<InputMapItem>();

            foreach (var type in excludedTypes)
                itemTypes.Remove(type);

            _creatableItems = itemTypes
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

                    return new CreatableItem(name, x);
                })
                .ToArray();
        }

        protected override void OnGUI(OnGUIContext context)
        {
            GUILayout.Label("Create", EditorStyles.whiteLargeLabel);

            foreach (var item in _creatableItems)
            {
                string name = item.name;
                if (GUILayout.Button($"Create New {name}"))
                {
                    InputMapItem mapItem = (InputMapItem)Activator.CreateInstance(item.type, new object[] { });
                    mapItem.ItemName = InputMapWindowEditorUtility.GenerateUniqueName("New item", 
                        s => NonRepeatableChecker.ContainsKey(map.groups[window.SelectedGroup].items, s));

                    window.AddItem(mapItem);
                }
            }
        }

        struct CreatableItem
        {
            public CreatableItem(string name, Type type)
            {
                this.name = name;
                this.type = type;
            }

            public string name;
            public Type type;
        }
    }
}
