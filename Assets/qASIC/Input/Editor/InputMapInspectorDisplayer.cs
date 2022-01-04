using UnityEditor;
using UnityEngine;
using System;

using static qASIC.UnityEditor.qGUIUtility;
using static UnityEngine.GUILayout;

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

        public event Action<InputGroup> OnDeleteGroup;
        public event Action<InspectorInputAction> OnDeleteAction;

        bool actionKeyFoldout = true;

        public void OnGUI()
        {
            scroll = BeginScrollView(scroll);

            switch (inspectionObject)
            {
                case InputGroup group:
                    group.groupName = EditorGUILayout.TextField("Name", group.groupName);

                    if (DeleteButton())
                    {
                        SetObject(null);
                        OnDeleteGroup.Invoke(group);
                    }

                    break;
                case InspectorInputAction action:

                    action.action.actionName = EditorGUILayout.TextField("Name", action.action.actionName);

                    DisplayKeys(action.action);
                    EditorGUILayout.Space();

                    if (DeleteButton())
                    {
                        SetObject(null);
                        OnDeleteAction.Invoke(action);
                    }
                    break;
                case InputAction _:
                    DrawMessageBox($"Use '{nameof(InspectorInputAction)}' instead of '{nameof(InputAction)}'!");
                    break;
            }

            EndScrollView();
        }

        public void SetObject(object obj)
        {
            inspectionObject = obj;
            displayDeletePrompt = false;
            GUI.FocusControl(null);
        }

        public string GetFoldoutPrefsKey(InspectorInputAction action) =>
            map ? $"qASIC_editor_input_{map.GetInstanceID()}_{action.group.groupName}_{action.action.actionName}" : string.Empty;

        public void DisplayKeys(InputAction action)
        {
            actionKeyFoldout = EditorGUILayout.Foldout(actionKeyFoldout, "Keys", true, EditorStyles.foldoutHeader);
            if (!actionKeyFoldout) return;

            BeginVertical(new GUIStyle() { margin = new RectOffset((int)EditorGUIUtility.singleLineHeight, 0, 0, 0) });

            for (int i = 0; i < action.keys.Count; i++)
            {
                BeginHorizontal();
                action.keys[i] = KeyCodePopup(action.keys[i], $"Key {i}");
                if (Button("Change", Width(60f))) { }
                EndHorizontal();
            }

            if (Button("+"))
                action.keys.Add(default);

            EndVertical();
        }

        public bool DeleteButton()
        {
            bool state = false;
            BeginHorizontal();
            switch (displayDeletePrompt)
            {
                case true:
                    if (Button("Cancel"))
                        displayDeletePrompt = false;

                    state = Button("Confirm");
                    if (state)
                        displayDeletePrompt = false;
                    break;
                case false:
                    if (Button("Delete"))
                        displayDeletePrompt = true;
                    break;
            }
            EndHorizontal();
            return state;
        }
    }
}