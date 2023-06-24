using UnityEditor;
using UnityEngine;

namespace qASIC.Displayer.Internal
{
    [CustomPropertyDrawer(typeof(DisplayerLine))]
    public class DisplayerLineDrawer : PropertyDrawer
    {
        const float _TAG_RECT_WIDTH = 90f;
        const float _VALUE_RECT_WIDTH = 128f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight * 2f +
            EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);

            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

            var tagProperty = property.FindPropertyRelative("tag");
            var textProperty = property.FindPropertyRelative("text");
            var valueProperty = property.FindPropertyRelative("value");
            var showProperty = property.FindPropertyRelative("show");

            Rect showRect = new Rect(position)
                .ResizeToLeft(EditorGUIUtility.singleLineHeight);

            Rect tagTextRect = new Rect(position)
                .BorderLeft(EditorGUIUtility.singleLineHeight)
                .ResizeToLeft(_TAG_RECT_WIDTH);

            Rect tagRect = new Rect(tagTextRect)
                .NextLine(false);

            Rect textTextRect = new Rect(position)
                .BorderLeft(_TAG_RECT_WIDTH + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)
                .BorderRight(_VALUE_RECT_WIDTH + EditorGUIUtility.standardVerticalSpacing);

            Rect textRect = new Rect(textTextRect)
                .NextLine(false);

            Rect valueTextRect = new Rect(position)
                .ResizeToRight(_VALUE_RECT_WIDTH);

            Rect valueRect = new Rect(valueTextRect)
                .NextLine(false);

            showRect = showRect
                .NextLine(false);


            EditorGUI.LabelField(tagTextRect, "Tag");
            EditorGUI.LabelField(textTextRect, "Text");
            EditorGUI.LabelField(valueTextRect, "Value");

            EditorGUI.PropertyField(showRect, showProperty, GUIContent.none);
            EditorGUI.PropertyField(tagRect, tagProperty, GUIContent.none);
            EditorGUI.PropertyField(textRect, textProperty, GUIContent.none);
            EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}