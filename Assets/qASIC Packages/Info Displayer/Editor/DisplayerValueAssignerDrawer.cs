using UnityEditor;
using UnityEngine;

namespace qASIC.Displayer.Internal
{
    [CustomPropertyDrawer(typeof(DisplayerValueAssigner))]
    public class DisplayerValueAssignerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            Rect toggleRect = new Rect(position)
                .ResizeToLeft(EditorGUIUtility.singleLineHeight);

            Rect tagRect = new Rect(position)
                .BorderLeft(EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(toggleRect, property.FindPropertyRelative("show"), GUIContent.none);
            EditorGUI.PropertyField(tagRect, property.FindPropertyRelative("tag"), GUIContent.none);
        }
    }
}