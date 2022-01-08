using UnityEditor;
using UnityEngine;
using System;

using static qASIC.EditorTools.qGUIUtility;
using static UnityEditor.EditorGUILayout;

namespace qASIC.InputManagement.Internal
{
    public class InputMapInspectorDisplayer
    {
        public InputMap map;

        object inspectionObject;
        bool displayDeletePrompt;

        Vector2 scroll;

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
            scroll = BeginScrollView(scroll);

            switch (inspectionObject)
            {
                case InputGroup group:
                    group.groupName = NameField(group.groupName);

                    if (DeleteButton())
                    {
                        ResetInspector();
                        OnDeleteGroup.Invoke(group);
                    }

                    break;
                case InspectorInputAction action:
                    action.action.actionName = NameField(action.action.actionName);

                    DisplayKeys(action.action);
                    Space();

                    if (DeleteButton())
                    {
                        ResetInspector();
                        OnDeleteAction.Invoke(action);
                    }
                    break;
                case InspectorInputAxis axis:
                    axis.axis.axisName = NameField(axis.axis.axisName);

                    axis.axis.positiveAction = TextField("Positive", axis.axis.positiveAction);
                    axis.axis.negativeAction = TextField("Negative", axis.axis.negativeAction);

                    if (DeleteButton())
                    {
                        ResetInspector();
                        OnDeleteAxis.Invoke(axis);
                    }
                    break;
                case InputAction _:
                    DrawMessageBox($"Use '{nameof(InspectorInputAction)}' instead of '{nameof(InputAction)}'!");
                    break;
                case InputAxis _:
                    DrawMessageBox($"Use '{nameof(InspectorInputAxis)}' instead of '{nameof(InputAxis)}'!");
                    break;
            }

            EndScrollView();

            if (Event.current.type == EventType.Repaint)
            {
                OnNextRepaint?.Invoke();
                OnNextRepaint = new Action(() => { });
            }
        }

        public void SetObject(object obj)
        {
            //If the object gets selected right now after the layout event
            //there could be a problem with instance IDs so in order to avoid
            //that we assign the object on next repaint
            OnNextRepaint += () =>
            {
                inspectionObject = obj;
                displayDeletePrompt = false;
            };
        }

        public void ResetInspector()
        {
            SetObject(null);
            GUI.FocusControl(null);
        }

        string GetFoldoutPrefsKey(InspectorInputAction action) =>
            map ? $"qASIC_editor_input_{map.GetInstanceID()}_{action.group.groupName}_{action.action.actionName}" : string.Empty;

        void DisplayKeys(InputAction action)
        {
            actionKeyFoldout = Foldout(actionKeyFoldout, "Keys", true, EditorStyles.foldoutHeader);
            if (!actionKeyFoldout) return;

            BeginVertical(new GUIStyle() { margin = new RectOffset((int)EditorGUIUtility.singleLineHeight, 0, 0, 0) });

            for (int i = 0; i < action.keys.Count; i++)
            {
                BeginHorizontal();
                action.keys[i] = KeyCodePopup(action.keys[i], $"Key {i}");
                if (GUILayout.Button("Change", GUILayout.Width(60f))) { }
                EndHorizontal();
            }

            if (GUILayout.Button("+"))
                action.keys.Add(default);

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
            switch (displayDeletePrompt)
            {
                case true:
                    if (GUILayout.Button("Cancel"))
                        displayDeletePrompt = false;

                    state = GUILayout.Button("Confirm");
                    if (state)
                        displayDeletePrompt = false;
                    break;
                case false:
                    if (GUILayout.Button("Delete"))
                        displayDeletePrompt = true;
                    break;
            }
            EndHorizontal();
            return state;
        }
    }
}