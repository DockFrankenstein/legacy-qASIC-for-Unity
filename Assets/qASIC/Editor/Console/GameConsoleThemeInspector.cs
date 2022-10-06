#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools.Internal;
using qASIC.EditorTools;
using System;

namespace qASIC.Console.Internal
{
    [CustomEditor(typeof(GameConsoleTheme))]
    public class GameConsoleThemeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool isReadOnly = (target as GameConsoleTheme).IsReadOnly;

            if (isReadOnly)
                EditorGUILayout.HelpBox("This is a read only theme. If you want to modify values, please create a new one.", MessageType.Warning);

#if !qASIC_DEV
            EditorGUI.BeginDisabledGroup(isReadOnly);
#endif

            qGUIInternalUtility.BeginGroup("Basic");
            qGUIEditorUtility.DrawPropertiesInRange(serializedObject, 
                nameof(GameConsoleTheme.DefaultColor), 
                nameof(GameConsoleTheme.InfoColor));
            qGUIInternalUtility.EndGroup();

            qGUIInternalUtility.BeginGroup("Tools");
            qGUIEditorUtility.DrawPropertiesInRange(serializedObject,
                nameof(GameConsoleTheme.qASICColor),
                nameof(GameConsoleTheme.InitColor));
            qGUIInternalUtility.EndGroup();

            qGUIInternalUtility.BeginGroup("Unity");
            qGUIEditorUtility.DrawPropertiesInRange(serializedObject,
                nameof(GameConsoleTheme.UnityAssertColor),
                nameof(GameConsoleTheme.UnityMessageColor));
            qGUIInternalUtility.EndGroup();

            qGUIInternalUtility.BeginGroup("Custom");
            DrawCustomColorArray();
            qGUIInternalUtility.EndGroup();

#if !qASIC_DEV
            EditorGUI.EndDisabledGroup();
#endif


#if qASIC_DEV
            DrawGroup("Other", new string[] { "isReadOnly" });
#endif

            serializedObject.ApplyModifiedProperties();
        }

        void DrawCustomColorArray()
        {
            SerializedProperty array = serializedObject.FindProperty("Colors");

            for (int i = 0; i < array.arraySize; i++)
            {
                SerializedProperty color = array.GetArrayElementAtIndex(i);

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(color);

                if (GUILayout.Button("-", new GUIStyle("Button") { margin = new RectOffset(0, 0, (int)EditorGUIUtility.singleLineHeight, 0) }, GUILayout.Width(EditorGUIUtility.singleLineHeight)))
                    array.DeleteArrayElementAtIndex(i);

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                GameConsoleTheme theme = (GameConsoleTheme)target;
                Array.Resize(ref theme.Colors, theme.Colors.Length + 1);
                theme.Colors[theme.Colors.Length - 1].color = Color.white;
                AssetDatabase.SaveAssets();
            }
        }

        void DrawGroup(string label, string[] properties) =>
            qGUIInternalUtility.DrawPropertyGroup(serializedObject, label, properties);
    }
}
#endif