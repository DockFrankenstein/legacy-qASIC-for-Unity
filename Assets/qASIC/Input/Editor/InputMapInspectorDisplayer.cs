﻿using UnityEditor;
using UnityEngine;
using System;
using qASIC.EditorTools;

using static qASIC.EditorTools.qGUIUtility;
using static UnityEditor.EditorGUILayout;

namespace qASIC.InputManagement.Internal
{
    public class InputMapInspectorDisplayer
    {
        public InputMap map;

        object _inspectionObject;
        bool _displayDeletePrompt;

        Vector2 _scroll;
        int _currentListeningKeyCode = -1;

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

        string nameFieldValue;

        event Action OnNextRepaint;

        public void OnGUI()
        {
            _scroll = BeginScrollView(_scroll);

            switch (_inspectionObject)
            {
                case InputGroup group:
                    group.groupName = NameField(group.groupName);

                    if (DeleteButton())
                        OnDeleteGroup.Invoke(group);

                    break;
                case InspectorInputAction action:
                    action.action.actionName = NameField(action.action.actionName);

                    DisplayKeys(action.action);
                    Space();

                    if (DeleteButton())
                        OnDeleteAction.Invoke(action);
                    break;
                case InspectorInputAxis axis:
                    axis.axis.axisName = NameField(axis.axis.axisName);

                    EditorChangeChecker.BeginChangeCheck(InputMapWindow.SetMapDirty);
                    axis.axis.positiveAction = TextField("Positive", axis.axis.positiveAction);
                    axis.axis.negativeAction = TextField("Negative", axis.axis.negativeAction);
                    EditorChangeChecker.EndChangeCheckAndCleanup();

                    if (DeleteButton())
                        OnDeleteAxis.Invoke(axis);
                    break;
                case InputAction _:
                    DrawMessageBox($"Use '{nameof(InspectorInputAction)}' instead of '{nameof(InputAction)}'!");
                    break;
                case InputAxis _:
                    DrawMessageBox($"Use '{nameof(InspectorInputAxis)}' instead of '{nameof(InputAxis)}'!");
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
            };
        }

        public void ResetInspector()
        {
            SetObject(null);
            GUI.FocusControl(null);
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

        string NameField(string name)
        {
            if (!EditorGUIUtility.editingTextField)
                nameFieldValue = name;

            nameFieldValue = TextField("Name", nameFieldValue);
            if (EditorGUIUtility.editingTextField)
                return name;

            if(nameFieldValue != name)
                InputMapWindow.SetMapDirty();

            return nameFieldValue;
        }

        bool DeleteButton()
        {
            bool state = false;
            BeginHorizontal();
            switch (_displayDeletePrompt)
            {
                case true:
                    if (GUILayout.Button("Cancel"))
                        _displayDeletePrompt = false;

                    if (state = GUILayout.Button("Confirm"))
                    {
                        _displayDeletePrompt = false;
                        ResetInspector();
                        InputMapWindow.SetMapDirty();
                    }
                    break;
                case false:
                    if (GUILayout.Button("Delete"))
                        _displayDeletePrompt = true;
                    break;
            }
            EndHorizontal();
            return state;
        }
    }
}