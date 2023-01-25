using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using qASIC.EditorTools;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class InputMapWindowSettingsInspector : InputMapItemInspector
    {
        public override bool AutoSave => false;

        bool _delete;

        ReorderableList _defaultBindingKeysReorderableList;
        List<string> _defaultBindingKeysList;

        public override void Initialize(OnInitializeContext context)
        {
            base.Initialize(context);
            _defaultBindingKeysList = new List<string>(InputMapWindow.Prefs_DefaultBindingKeys);
            _defaultBindingKeysReorderableList = new ReorderableList(_defaultBindingKeysList, typeof(string), true, true, true, true);
            _defaultBindingKeysReorderableList.drawHeaderCallback += (rect) =>
            {
                GUI.Label(rect, "Default Binding Key List");
            };
            _defaultBindingKeysReorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                using (new EditorChangeChecker.ChangeCheck(() => InputMapWindow.Prefs_DefaultBindingKeys = _defaultBindingKeysList))
                {
                    _defaultBindingKeysReorderableList.list[index] = EditorGUI.DelayedTextField(rect, _defaultBindingKeysList[index]);
                }
            };
            _defaultBindingKeysReorderableList.onChangedCallback += (list) =>
            {
                InputMapWindow.Prefs_DefaultBindingKeys = _defaultBindingKeysList;
                Debug.Log("Changed");
            };
            _defaultBindingKeysReorderableList.onAddCallback += (list) =>
            {
                list.list.Add(string.Empty);
                Debug.Log("Add");
            };
        }

        protected override void OnGUI(OnGUIContext context)
        {
            GUILayout.Label("Settings", EditorStyles.whiteLargeLabel);

            InputMapWindow.Prefs_AutoSave = Toggle("Auto Save", InputMapWindow.Prefs_AutoSave);
            InputMapWindow.Prefs_AutoSaveTimeLimit = FloatField("Auto Save Time Limit", InputMapWindow.Prefs_AutoSaveTimeLimit);
            Space();
            InputMapWindow.Prefs_DefaultGroupColor = ColorField("Default Group Color", InputMapWindow.Prefs_DefaultGroupColor);
            Space();
            InputMapWindow.Prefs_ShowItemIcons = Toggle("Show Item Icons", InputMapWindow.Prefs_ShowItemIcons);
            Space();
            _defaultBindingKeysReorderableList.DoLayoutList();

            Space();

            _delete = DeleteButton(_delete, "Reset preferences", InputMapWindow.ResetPreferences);
        }

        protected override void OnDebugGUI(OnGUIContext context)
        {
            base.OnDebugGUI(context);
            InputMapWindow.Prefs_InspectorWidth = FloatField("Inspector Width", InputMapWindow.Prefs_InspectorWidth);
        }
    }
}
