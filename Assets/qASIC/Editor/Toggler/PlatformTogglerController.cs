#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.EditorTools.Internal;

namespace qASIC.Toggling.Controllers.Internal
{
    [CustomEditor(typeof(PlatformTogglerController))]
    public class PlatformTogglerControllerInspector : Editor
    {
        PlatformTogglerController _controller;

        SerializedProperty p_defaultToggler;
        SerializedProperty p_platformTogglers;

        private void OnEnable()
        {
            _controller = (PlatformTogglerController)target;

            p_defaultToggler = serializedObject.FindProperty("defaultToggler");
            p_platformTogglers = serializedObject.FindProperty("platformTogglers");

            _controller.FixPlatformTogglers();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(p_defaultToggler);

            EditorGUILayout.Space();
            qGUIInternalUtility.BeginGroup();

            for (int i = 0; i < p_platformTogglers.arraySize; i++)
            {
                SerializedProperty item = p_platformTogglers.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(item.FindPropertyRelative("toggler"),
                    new GUIContent(((RuntimePlatform)item.FindPropertyRelative("platform").intValue).ToString()));
            }

            qGUIInternalUtility.EndGroup();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif