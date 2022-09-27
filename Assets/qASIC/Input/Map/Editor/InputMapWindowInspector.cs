#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using qASIC.EditorTools;
using qASIC.EditorTools.Internal;
using UnityEditorInternal;
using qASIC.InputManagement.Internal.KeyProviders;
using qASIC.InputManagement.Map.Internal.Inspectors;
using qASIC.Tools;

using static qASIC.EditorTools.qGUIEditorUtility;
using static UnityEditor.EditorGUILayout;
using UnityEditor.PackageManager.UI;

namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapWindowInspector
    {
        public InputMap map;
        public InputMapWindow window;

        InputMapItem _inspectionObject;
        bool _displayDeletePrompt;

        Vector2 _scroll;

        public string DefaultText { get; set; }

        private Dictionary<Type, InputMapItemInspector> _inspectors = null;
        public Dictionary<Type, InputMapItemInspector> _Inspectors
        {
            get
            {
                if (_inspectors == null)
                {
                    _inspectors = TypeFinder.CreateConstructorsFromTypesList<InputMapItemInspector>(TypeFinder.FindAllTypes<InputMapItemInspector>())
                        .ToDictionary(x => x.ItemType);
                }

                return _inspectors;
            }
        }

        bool _resetNameField = false;
        bool _editingNameField = false;

        InputMapItemInspector _currentInspector;
        InputMapItemInspector.DefaultGUIData _currentInspectorDefaultGUIData;

        string nameFieldValue;

        event Action OnNextRepaint;

        #region GUI
        public void OnGUI()
        {
            if (!map) return;

            _scroll = BeginScrollView(_scroll);

            InputMapItemInspector.OnGUIContext context = new InputMapItemInspector.OnGUIContext()
            {
                item = _inspectionObject,
                debug = InputMapWindow.DebugMode,
            };
            //context.serializedObject = new SerializedObject(_inspectionObject);

            _currentInspector?.DrawGUI(context);


            //switch (_inspectionObject)
            //{
            //    case InputGroup group:
            //        int groupIndex = map.groups.IndexOf(group);
            //        if (groupIndex == -1) break;

            //        group.groupName = NameField(group.groupName, (string newName) => { return map.CanRenameGroup(newName); });

            //        using (new EditorGUI.DisabledScope(map.defaultGroup == groupIndex))
            //        {
            //            if (GUILayout.Button("Set as default"))
            //            {
            //                map.defaultGroup = groupIndex;
            //                _displayDeletePrompt = false;
            //                InputMapWindow.SetMapDirty();
            //            }

            //            Space();
            //            if (DeleteButton())
            //                OnDeleteGroup?.Invoke(group);
            //        }


            //        //Debug
            //        if (InputMapWindow.DebugMode)
            //        {
            //            Space();
            //            qGUIInternalUtility.BeginGroup();
            //            EditorChangeChecker.BeginChangeCheck(InputMapWindow.SetMapDirty);

            //            group.guid = DelayedTextField("GUID", group.guid);

            //            EditorChangeChecker.EndChangeCheckAndCleanup();
            //            qGUIInternalUtility.EndGroup(false);
            //        }
            //        break;
            //    case InspectorInputAction action:
            //        action.action.actionName = NameField(action.action.actionName, (string newName) => { return action.group.CanRenameItem(newName); });

            //        DisplayKeys(action.action);
            //        Space();

            //        if (DeleteButton())
            //            OnDeleteAction?.Invoke(action);

            //        //Debug
            //        if (InputMapWindow.DebugMode)
            //        {
            //            Space();
            //            qGUIInternalUtility.BeginGroup();
            //            EditorChangeChecker.BeginChangeCheck(InputMapWindow.SetMapDirty);

            //            action.action.guid = DelayedTextField("GUID", action.action.guid);

            //            EditorChangeChecker.EndChangeCheckAndCleanup();
            //            qGUIInternalUtility.EndGroup(false);
            //        }
            //        break;
            //    case InspectorInputAxis axis:
            //        axis.axis.axisName = NameField(axis.axis.axisName, (string newName) => { return axis.group.CanRenameAxis(newName); });

            //        Space();

            //        EditorChangeChecker.BeginChangeCheck(InputMapWindow.SetMapDirty);
            //        axis.axis.positiveAction = AxisField(axis.group, "Positive", axis.axis.positiveAction);
            //        axis.axis.negativeAction = AxisField(axis.group, "Negative", axis.axis.negativeAction);
            //        EditorChangeChecker.EndChangeCheckAndCleanup();

            //        Space();

            //        if (DeleteButton())
            //            OnDeleteAxis?.Invoke(axis);

            //        //Debug
            //        if (InputMapWindow.DebugMode)
            //        {
            //            Space();
            //            qGUIInternalUtility.BeginGroup();
            //            EditorChangeChecker.BeginChangeCheck(InputMapWindow.SetMapDirty);

            //            axis.axis.guid = DelayedTextField("GUID", axis.axis.guid);

            //            EditorChangeChecker.EndChangeCheckAndCleanup();
            //            qGUIInternalUtility.EndGroup(false);
            //        }
            //        break;
            //    case InputBinding _:
            //        HelpBox(new GUIContent($"Use '{nameof(InspectorInputAction)}' instead of '{nameof(InputBinding)}'!"));
            //        break;
            //    case Input1DAxis _:
            //        HelpBox(new GUIContent($"Use '{nameof(InspectorInputAxis)}' instead of '{nameof(Input1DAxis)}'!"));
            //        break;
            //    case string s:
            //        HandleStringInspection(s);
            //        break;
            //    default:
            //        GUILayout.Label(DefaultText);
            //        break;
            //}

            EndScrollView();

            if (OnNextRepaint != null && Event.current.type == EventType.Repaint)
            {
                OnNextRepaint?.Invoke();
                OnNextRepaint = null;
                InputMapWindow.GetEditorWindow().Repaint();
            }
        }

        void HandleStringInspection(string s)
        {
            switch (s)
            {
                case "settings":
                    GUILayout.Label("Settings", EditorStyles.whiteLargeLabel);

                    InputMapWindow.Prefs_AutoSave = Toggle("Auto Save", InputMapWindow.Prefs_AutoSave);
                    InputMapWindow.Prefs_AutoSaveTimeLimit = FloatField("Auto Save Time Limit", InputMapWindow.Prefs_AutoSaveTimeLimit);
                    Space();
                    InputMapWindow.Prefs_DefaultGroupColor = ColorField("Default Group Color", InputMapWindow.Prefs_DefaultGroupColor);
                    Space();
                    InputMapWindow.Prefs_ShowItemIcons = Toggle("Show Item Icons", InputMapWindow.Prefs_ShowItemIcons);

                    if (InputMapWindow.DebugMode)
                    {
                        Space();
                        GUILayout.Label("Debug", EditorStyles.largeLabel);
                        InputMapWindow.Prefs_InspectorWidth = FloatField("Inspector Width", InputMapWindow.Prefs_InspectorWidth);
                    }

                    Space();

                    if (DeleteButton("Reset preferences"))
                        InputMapWindow.ResetPreferences();
                    break;
                default:
                    HelpBox($"Message {s} not recognized!", MessageType.Error);
                    break;
            }
        }


        Dictionary<Type, ReorderableList> _keysReorderableLists;
        void DisplayKeys(InputBinding action)
        {
            //EditorGUILayout.Space();
            //BeginVertical(new GUIStyle() { margin = new RectOffset(4, 4, 0, 0) });
            //EditorChangeChecker.BeginChangeCheck(InputMapWindow.SetMapDirty);


            //Dictionary<Type, KeyTypeProvider> providers = InputMapEditorUtility.KeyTypeProvidersDictionary;

            //foreach (InputBinding.KeyList list in action.keys)
            //{
            //    Type type = Type.GetType(list.keyType);

            //    if (type == null || !providers.ContainsKey(type))
            //    {
            //        HelpBox($"Type '{list.keyType}' cannot be recognized!", MessageType.Error);
            //        Space();
            //        continue;
            //    }

            //    _keysReorderableLists[type].DoLayoutList();
            //    Space();
            //}


            //EditorChangeChecker.EndChangeCheckAndCleanup();
            //EndVertical();
        }
        #endregion

        #region Control
        public void SetObject(InputMapItem obj)
        {
            //If the object gets selected right now after the layout event
            //there could be a problem with instance IDs so in order to avoid
            //that we assign the object on next repaint
            OnNextRepaint += () =>
            {
                _inspectionObject = obj;
                _displayDeletePrompt = false;
                if (_editingNameField)
                    GUI.FocusControl(null);
                _resetNameField = true;

                Type type = obj?.GetType();
                _currentInspector = null;

                if (type != null && _Inspectors.ContainsKey(type))
                {
                    InputMapItemInspector.OnInitializeContext context = new InputMapItemInspector.OnInitializeContext()
                    {
                        item = obj,
                    };

                    _currentInspector = _Inspectors[type];
                    _currentInspector.window = window;
                    _currentInspector.map = map;
                    _currentInspector.Initialize(context);
                }

                _currentInspectorDefaultGUIData = new InputMapItemInspector.DefaultGUIData()
                {
                    map = map,
                    window = window,
                };

                //Initialize for specific types
                //switch (obj)
                //{
                //    case InspectorInputAction action:
                //        _keysReorderableLists = new Dictionary<Type, ReorderableList>();
                //        Dictionary<Type, KeyTypeProvider> providers = InputMapEditorUtility.KeyTypeProvidersDictionary;

                //        foreach (InputBinding.KeyList list in action.action.keys)
                //        {
                //            Type type = Type.GetType(list.keyType);

                //            if (type == null || !providers.ContainsKey(type) || _keysReorderableLists.ContainsKey(type))
                //                continue;

                //            ReorderableList reorderableList = new ReorderableList(list.keys, typeof(int), true, true, true, true);
                //            reorderableList.drawHeaderCallback += (rect) => EditorGUI.LabelField(rect, providers[type].DisplayName);
                //            reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
                //            {
                //                list.keys[index] = providers[type].OnPopupGUI(rect, list.keys[index], isActive, isFocused);
                //            };

                //            _keysReorderableLists.Add(type, reorderableList);
                //        }
                //        break;
                //}
            };
        }

        public void ResetInspector()
        {
            SetObject(null);
            GUI.FocusControl(null);
            _resetNameField = true;
        }
        #endregion

        #region GUI Methods
        string NameField(string name, bool setDirty = true) =>
            NameField(name, _ => { return true; }, setDirty);

        string NameField(string text, Func<string, bool> canRename, bool setDirty = true)
        {
            if (!_editingNameField || _resetNameField || !EditorGUIUtility.editingTextField)
            {
                nameFieldValue = text;
                _resetNameField = false;
                _editingNameField = false;
            }

            nameFieldValue = TextField("Name", nameFieldValue);
            _editingNameField = EditorGUIUtility.editingTextField;
            if (_editingNameField || !canRename.Invoke(nameFieldValue))
                return text;

            if (setDirty && nameFieldValue != text)
            {
                window.SetMapDirty();
                window.ReloadTreesNextRepaint();
            }

            return nameFieldValue;
        }

        string AxisField(InputGroup group, string label, string value)
        {
            GUIContent guiContent = new GUIContent(label);
            if (!group.ItemExists(value))
            {
                guiContent.image = ErrorIcon;
                guiContent.tooltip = "This action doesn't exist";
            }

            return DelayedTextField(guiContent, value);
        }

        bool DeleteButton(string label = "Delete", string confirm = "Confirm", string cancel = "Cancel")
        {
            bool state = false;
            BeginHorizontal();
            switch (_displayDeletePrompt)
            {
                case true:
                    if (GUILayout.Button(cancel))
                        _displayDeletePrompt = false;

                    if (state = GUILayout.Button(confirm))
                    {
                        _displayDeletePrompt = false;
                        ResetInspector();
                        window.SetMapDirty();
                    }
                    break;
                case false:
                    if (GUILayout.Button(label))
                        _displayDeletePrompt = true;
                    break;
            }
            EndHorizontal();
            return state;
        }
        #endregion
    }
}
#endif