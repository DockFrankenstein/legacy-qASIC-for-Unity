using qASIC.EditorTools.Internal;
using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using qASIC.InputManagement.Internal.KeyProviders;
using System.Linq;

namespace qASIC.InputManagement.Map.Internal.Inspectors
{
    public class InputBindingInspector : InputMapItemInspector
    {
        public override Type ItemType => typeof(InputBinding);

        SerializedProperty p_itemName;
        SerializedProperty p_keys;
        SerializedProperty p_guid;

        GUIContent c_guid;

        GUIStyle keysStyle;

        InputBinding _binding;
        List<InputBinding.KeyList> _keyLists;

        bool _delete;
        Dictionary<string, ReorderableList> _keysReorderableLists;

        ReorderableList _keyPathReorderableList;

        Dictionary<string, List<string>> _keys;

        public override void Initialize(OnInitializeContext context)
        {
            _binding = context.item as InputBinding;
            _keyLists = _binding.GetSortedKeys();

            //p_itemName = context.serializedObject.FindProperty(nameof(InputBinding.itemName));
            //p_keys = context.serializedObject.FindProperty(nameof(InputBinding.keys));
            //p_guid = context.serializedObject.FindProperty(nameof(InputBinding.guid));

            c_guid = new GUIContent("GUID");

            keysStyle = new GUIStyle()
            {
                margin = new RectOffset(4, 4, 0, 0),
            };

            _keyPathReorderableList = new ReorderableList(_binding.keys, typeof(string), true, true, true, true);
            _keyPathReorderableList.drawHeaderCallback += (rect) => EditorGUI.LabelField(rect, "Key paths");
            _keyPathReorderableList.onAddCallback += _ =>
            {
                _binding.keys.Add(string.Empty);
            };

            _keyPathReorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                _binding.keys[index] = EditorGUI.DelayedTextField(rect, _binding.keys[index]);
            };

            //_keysReorderableLists = new Dictionary<string, ReorderableList>();
            //var providers = InputMapEditorUtility.KeyTypeProvidersDictionary;

            //foreach (var provider in providers)
            //{
            //    List<string> list = _keyLists.Where(x => x.keyName == provider.Key).First().keys.Select(x => x.path);

            //    ReorderableList reorderableList = new ReorderableList(listItems, typeof(int), true, true, true, true);
            //    reorderableList.drawHeaderCallback += (rect) => EditorGUI.LabelField(rect, provider.Value.DisplayName);
            //    reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
            //    {
            //        list.keys[index] = providers[type].OnPopupGUI(rect, list.keys[index], isActive, isFocused);
            //    };

            //    _keysReorderableLists.Add(type, reorderableList);
            //}
        }

        protected override void OnGUI(OnGUIContext context)
        {
            DisplayKeys();

            //DisplayKeys();
            //EditorGUILayout.Space();
        }

        void DisplayKeys()
        {
            EditorGUILayout.Space();

            _keyPathReorderableList.DoLayoutList();

            //using (new GUILayout.VerticalScope(keysStyle))
            //{
            //    Dictionary<string, KeyTypeProvider> providers = InputMapEditorUtility.KeyTypeProvidersDictionary;

            //    foreach (var providerList in providers)
            //    {
            //        providersList
            //    }
            //}
        }

        protected override void HandleDeletion(OnGUIContext context)
        {
            var group = map.groups
                .Where(x => x.items.Contains(context.item))
                .First()
                .items
                .Remove(context.item as InputMapItem);
        }
    }
}
