using UnityEditor;
using UnityEngine;
using qASIC.InputManagement.EditorTools;

using static UnityEditor.EditorGUILayout;

namespace qASIC.InputManagement.Internal
{
    public class InputReferenceExplorerWindow : EditorWindow
    {
        public static SerializedProperty Property { get; private set; }

        int groupScrollIndex;
        int selectedGroup;

        public static void OpenProperty(SerializedProperty property)
        {
            Property = property;
            OpenWindow();
        }

        public static InputReferenceExplorerWindow OpenWindow()
        {
            InputReferenceExplorerWindow window = GetEditorWindow();
            window.minSize = new Vector2(256f, 256f);
            window.titleContent = new GUIContent("Input Reference Explorer");
            window.ShowPopup();
            return window;
        }

        public static InputReferenceExplorerWindow GetEditorWindow() =>
            (InputReferenceExplorerWindow)GetWindow(typeof(InputReferenceExplorerWindow), false, "Input Map Editor");

        public void OnGUI()
        {
            if(!EditorInputManager.Map)
            {
                HelpBox("Please open an Input Map in the Input Map Editor before using!", MessageType.Info);
                return;
            }

            BeginHorizontal(EditorStyles.toolbar);

            MoveButton('<', -1, groupScrollIndex - 1 >= 0);

            BeginScrollView(Vector2.zero, GUIStyle.none, GUIStyle.none);
            BeginHorizontal(EditorStyles.toolbar);

            for (int i = groupScrollIndex; i < EditorInputManager.Map.Groups.Count; i++)
            {
                GUIContent buttonContent = new GUIContent(EditorInputManager.Map.Groups[i].groupName);
                EditorStyles.toolbarButton.CalcMinMaxWidth(buttonContent, out float width, out _);

                bool selected = selectedGroup == i;

                if (GUILayout.Toggle(selected, buttonContent, EditorStyles.toolbarButton, GUILayout.Width(width)) != selected)
                    selectedGroup = i;
            }

            EndHorizontal();
            EndScrollView();

            MoveButton('>', 1, EditorInputManager.Map && groupScrollIndex + 1 < EditorInputManager.Map.Groups.Count);

            EndHorizontal();
        }

        void MoveButton(char character, int value, bool canScroll)
        {
            EditorGUI.BeginDisabledGroup(!EditorInputManager.Map || !canScroll);
            if (GUILayout.Button(character.ToString(), EditorStyles.toolbarButton, GUILayout.Width(EditorGUIUtility.singleLineHeight)))
                groupScrollIndex += value;
            EditorGUI.EndDisabledGroup();
        }
    }
}