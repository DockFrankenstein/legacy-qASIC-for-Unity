#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.InputManagement.Map;
using qASIC.InputManagement.Map.Internal;
using System.Collections.Generic;
using qASIC.Tools;
using System;

using static UnityEditor.EditorGUILayout;
using Manager = qASIC.InputManagement.Internal.EditorInputManager;

namespace qASIC.InputManagement.Internal.ReferenceExplorers
{
    public abstract class InputReferenceExplorerBase : EditorWindow
    {
        public SerializedProperty Property { get; protected set; }

        protected SerializedProperty groupProperty;
        protected SerializedProperty useDefaultProperty;
        protected SerializedProperty contentProperty;

        protected InputMapGroupBar groupBar = new InputMapGroupBar();

        Vector2 contentScroll;
        protected int selectedItem = -1;

        protected static void OpenWindow(EditorWindow window)
        {
            window.minSize = new Vector2(300f, 400f);
            window.titleContent = new GUIContent("Input Reference Explorer");
            window.ShowAuxWindow();
        }

        public static InputActionReferenceExplorerWindow GetEditorWindow() =>
            (InputActionReferenceExplorerWindow)GetWindow(typeof(InputActionReferenceExplorerWindow), false, "Input Map Editor");

        public abstract List<INonRepeatable> GetContentList(int groupIndex);
        public abstract string ContentPropertyName { get; }

        protected void ResetEditor()
        {
            groupProperty = Property.FindPropertyRelative("groupName");
            useDefaultProperty = Property.FindPropertyRelative("useDefaultGroup");
            contentProperty = Property.FindPropertyRelative(ContentPropertyName);

            SelectCurrentProperties();

            groupBar.OnItemSelect += (object o) =>
            {
                InputGroup group = o as InputGroup;
                if (group.groupName == groupProperty.stringValue) return;
                groupProperty.stringValue = group.groupName;
                selectedItem = -1;
            };
        }

        void SelectCurrentProperties()
        {
            if (!Manager.Map || Manager.Map.Groups.Count == 0) return;

            int currentGroup = NonRepeatableChecker.GetNameList(Manager.Map.Groups).IndexOf(groupProperty.stringValue.ToLower());
            if (currentGroup == -1) return;

            groupBar.Select(currentGroup);
            selectedItem = NonRepeatableChecker.GetNameList(GetContentList(currentGroup)).IndexOf(contentProperty.stringValue.ToLower());
        }

        public void OnGUI()
        {
            if (Property == null)
            {
                Space();
                LabelField("No property selected", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            if (!Manager.Map)
            {
                HelpBox("Input Map not loaded - Please select an Input Map in project settings.", MessageType.Warning);
                if (GUILayout.Button("Open project settings"))
                    SettingsService.OpenProjectSettings("Project/qASIC/Input");
                return;
            }

            groupBar.SetMap(Manager.Map);

            BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            DrawDefaultGroupButton();
            Space(8f, false);
            EndHorizontal();

            EditorGUI.BeginDisabledGroup(useDefaultProperty.boolValue);
            groupBar.OnGUI();
            EditorGUI.EndDisabledGroup();

            DisplayContent();

            Space();
            EditorGUI.BeginDisabledGroup(selectedItem == -1);
            bool apply = GUILayout.Button("Apply");
            EditorGUI.EndDisabledGroup();

            List<INonRepeatable> content = GetContentList(groupBar.SelectedGroupIndex);

            Event e = Event.current;

            if (e.isKey && e.keyCode == KeyCode.Return && selectedItem != -1)
                apply = true;

            KeyEvent(KeyCode.UpArrow, selectedItem -1 >= 0, () => { selectedItem--; });
            KeyEvent(KeyCode.DownArrow, selectedItem + 1 < content.Count, () => { selectedItem++; });
            KeyEvent(KeyCode.LeftArrow, true, groupBar.SelectPrevious);
            KeyEvent(KeyCode.RightArrow, true, groupBar.SelectNext);

            if (apply)
            {
                if(groupBar.SelectedGroupIndex != -1)
                    groupProperty.stringValue = Manager.Map.Groups[groupBar.SelectedGroupIndex].groupName;

                contentProperty.stringValue = content[selectedItem].ItemName;
                Property.serializedObject.ApplyModifiedProperties();
                Close();
            }
        }

        void KeyEvent(KeyCode key, bool condition, Action action)
        {
            if (Event.current.type != EventType.KeyDown || Event.current.keyCode != key || !condition) return;
            action?.Invoke();
            Repaint();
        }

        void DrawDefaultGroupButton()
        {
            GUIContent buttonContent = new GUIContent("Use default");
            EditorStyles.toolbarButton.CalcMinMaxWidth(buttonContent, out float buttonWidth, out _);
            bool value = GUILayout.Toggle(useDefaultProperty.boolValue, buttonContent, EditorStyles.toolbarButton, GUILayout.Width(buttonWidth));

            if (value && !useDefaultProperty.boolValue)
                groupBar.Select(Manager.Map.defaultGroup);

            useDefaultProperty.boolValue = value;
        }

        void DisplayContent()
        {
            List<INonRepeatable> content = GetContentList(groupBar.SelectedGroupIndex);

            GUIContent[] contentNames = new GUIContent[content.Count];
            for (int i = 0; i < content.Count; i++)
                contentNames[i] = new GUIContent(content[i].ItemName);

            contentScroll = BeginScrollView(contentScroll);
            selectedItem = GUILayout.SelectionGrid(selectedItem, contentNames, 1, Styles.ActionStyle);

            EndScrollView();
        }

        static class Styles
        {
            public static GUIStyle ActionStyle => new GUIStyle(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft, };
        }
    }
}

#endif