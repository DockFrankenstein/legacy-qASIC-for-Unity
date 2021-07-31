using UnityEditor;
using UnityEngine;

namespace qASIC.Displayer.UnityEditor
{
    [CustomPropertyDrawer(typeof(DisplayerLine))]
    public class DisplayerLineDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.PropertyField(new Rect(position.x, position.y, 18, position.height), property.FindPropertyRelative("show"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(position.x + 22, position.y, 71, position.height), property.FindPropertyRelative("tag"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(position.x + 101, position.y, position.width - 165, position.height), property.FindPropertyRelative("text"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(position.x + position.width - 60, position.y, 60, position.height), property.FindPropertyRelative("value"), GUIContent.none);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}