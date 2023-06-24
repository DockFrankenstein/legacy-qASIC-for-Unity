#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools;
using System;
using System.Linq;

using UnityObject = UnityEngine.Object;

namespace qASIC.Input.UIM.Internal
{
    [CustomEditor(typeof(UIMAxisMapper))]
    public class UIMAxisMapperInspector : Editor
    {
        SerializedProperty p_defaultMapper;
        SerializedProperty p_windowsMapper;
        SerializedProperty p_linuxMapper;
        SerializedProperty p_macMapper;

        private void OnEnable()
        {
            p_defaultMapper = serializedObject.FindProperty(nameof(UIMAxisMapper.defaultMapper));
            p_windowsMapper = serializedObject.FindProperty(nameof(UIMAxisMapper.windowsMapper));
            p_linuxMapper = serializedObject.FindProperty(nameof(UIMAxisMapper.linuxMapper));
            p_macMapper = serializedObject.FindProperty(nameof(UIMAxisMapper.macMapper));
        }

        public override void OnInspectorGUI()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(p_defaultMapper, new GUIContent("Default"));
            }

            EditorGUILayout.Space();
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Platform mappers", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(p_windowsMapper, new GUIContent("Windows"));
                EditorGUILayout.PropertyField(p_linuxMapper, new GUIContent("Linux"));
                EditorGUILayout.PropertyField(p_macMapper, new GUIContent("Mac"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif