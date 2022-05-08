#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using qASIC.EditorTools;

using static qASIC.EditorTools.qGUIEditorUtility;
using static UnityEditor.EditorGUILayout;

namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapWindowInspector
    {
        public InputMap map;

        object _inspectionObject;
        bool _displayDeletePrompt;

        Vector2 _scroll;
        int _currentListeningKeyCode = -1;

        public string DefaultText { get; set; }

        public struct InspectorInputAction
        {
            public InputGroup group;
            public InputAction action;

            public InspectorInputAction(InputGroup group, InputAction action)
            {
                this.group = group;
                this.action = action;
            }
        }

        public struct InspectorInputAxis
        {
            public InputGroup group;
            public InputAxis axis;

            public InspectorInputAxis(InputGroup group, InputAxis axis)
            {
                this.group = group;
                this.axis = axis;
            }
        }

        public event Action<InputGroup> OnDeleteGroup;
        public event Action<InspectorInputAction> OnDeleteAction;
        public event Action<InspectorInputAxis> OnDeleteAxis;

        bool actionKeyFoldout = true;
        bool _resetNameField = false;
        bool _editingNameField = false;

        string nameFieldValue;

        event Action OnNextRepaint;

        #region GUI
        public void OnGUI()
        {
            if (!map) return;

            _scroll = BeginScrollView(_scroll);

            switch (_inspectionObject)
            {
                case InputGroup group:
                    int groupIndex = map.Groups.IndexOf(group);
                    if (groupIndex == -1) break;

                    group.groupName = NameField(group.groupName, (string newName) => { return map.CanRenameGroup(newName); });

                    EditorGUI.BeginDisabledGroup(map.defaultGroup == groupIndex);
                    if (GUILayout.Button("Set as default"))
                    {
                        map.defaultGroup = groupIndex;
                        _displayDeletePrompt = false;
                        InputMapWindow.SetMapDirty();
                    }

                    Space();
                    if (DeleteButton())
                        OnDeleteGroup?.Invoke(group);

                    EditorGUI.EndDisabledGroup();
                    break;
                case InspectorInputAction action:
                    action.action.actionName = NameField(action.action.actionName, (string newName) => { return action.group.CanRenameAction(newName); });

                    DisplayKeys(action.action);
                    Space();

                    if (DeleteButton())
                        OnDeleteAction?.Invoke(action);
                    break;
                case InspectorInputAxis axis:
                    axis.axis.axisName = NameField(axis.axis.axisName, (string newName) => { return axis.group.CanRenameAxis(newName); });

                    Space();

                    EditorChangeChecker.BeginChangeCheck(InputMapWindow.SetMapDirty);
                    axis.axis.positiveAction = AxisField(axis.group, "Positive", axis.axis.positiveAction);
                    axis.axis.negativeAction = AxisField(axis.group, "Negative", axis.axis.negativeAction);
                    EditorChangeChecker.EndChangeCheckAndCleanup();

                    Space();

                    if (DeleteButton())
                        OnDeleteAxis?.Invoke(axis);
                    break;
                case InputAction _:
                    HelpBox(new GUIContent($"Use '{nameof(InspectorInputAction)}' instead of '{nameof(InputAction)}'!"));
                    break;
                case InputAxis _:
                    HelpBox(new GUIContent($"Use '{nameof(InspectorInputAxis)}' instead of '{nameof(InputAxis)}'!"));
                    break;
                case string s:
                    HandleStringInspection(s);
                    break;
                default:
                    GUILayout.Label(DefaultText);
                    break;
            }

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

        void DisplayKeys(InputAction action)
        {
            actionKeyFoldout = Foldout(actionKeyFoldout, "Keys", true, EditorStyles.foldoutHeader);
            if (!actionKeyFoldout) return;

            BeginVertical(new GUIStyle() { margin = new RectOffset((int)EditorGUIUtility.singleLineHeight, 0, 0, 0) });

            //Checking for changes
            EditorChangeChecker.BeginChangeCheck(InputMapWindow.SetMapDirty);

            for (int i = 0; i < action.keys.Count; i++)
            {
                //Listening for key
                if (_currentListeningKeyCode == i)
                {
                    if (Event.current.isKey)
                    {
                        action.keys[i] = Event.current.keyCode;
                        _currentListeningKeyCode = -1;
                        InputMapWindow.SetMapDirty();
                        InputMapWindow.GetEditorWindow().Repaint();
                    }

                    if (EditorChangeChecker.IgnorableButton("Cancel") || Event.current.isMouse)
                    {
                        _currentListeningKeyCode = -1;
                        InputMapWindow.GetEditorWindow().Repaint();
                    }
                    continue;
                }

                //Drawing normal line
                BeginHorizontal();
                action.keys[i] = KeyCodePopup(action.keys[i]);

                if (EditorChangeChecker.IgnorableButton("Change", GUILayout.Width(60f)))
                {
                    _currentListeningKeyCode = i;
                    GUI.FocusControl(null);
                }

                if (GUILayout.Button("-", GUILayout.Width(EditorGUIUtility.singleLineHeight)))
                    action.keys.RemoveAt(i);

                EndHorizontal();
            }

            if (GUILayout.Button("+"))
                action.keys.Add(default);

            EditorChangeChecker.EndChangeCheckAndCleanup();
            EndVertical();
        }
        #endregion

        #region Control
        public void SetObject(object obj)
        {
            //If the object gets selected right now after the layout event
            //there could be a problem with instance IDs so in order to avoid
            //that we assign the object on next repaint
            OnNextRepaint += () =>
            {
                _inspectionObject = obj;
                _displayDeletePrompt = false;
                _currentListeningKeyCode = -1;
                if (_editingNameField)
                    GUI.FocusControl(null);
                _resetNameField = true;
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
                InputMapWindow.SetMapDirty();
                InputMapWindow.GetEditorWindow().ReloadTreesNextRepaint();
            }

            return nameFieldValue;
        }

        string AxisField(InputGroup group, string label, string value)
        {
            GUIContent guiContent = new GUIContent(label);
            if (!group.ActionExists(value))
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
                        InputMapWindow.SetMapDirty();
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