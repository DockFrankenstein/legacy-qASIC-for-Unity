using UnityEditor;
using UnityEngine;

namespace qASIC.Displayer.Editor
{
    [CustomPropertyDrawer(typeof(DisplayerLine))]
    public class DisplayerLineDrawer : PropertyDrawer
    {
        Rect baseRect;
        Rect currentRect;

        float spaces = 16f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            baseRect = position;
            baseRect.width -= (spaces * 4f);
            baseRect.height = 18f;
            currentRect = baseRect;

            DrawPropertyScale(property, 0.1f, 42f, "show", "");
            DrawPropertyScale(property, 0.2f, 20f, "tag", "tag");
            DrawPropertyScale(property, 0.5f, 25f, "text", "text");
            DrawPropertyScale(property, 0.2f, 34f, "value", "value");
            EditorGUI.EndProperty();
        }

        void DrawPropertyScale(SerializedProperty property, float widthScale, float labelWidth, string propertyName, string label) =>
            DrawProperty(property, baseRect.width * widthScale, labelWidth, propertyName, label);

        void DrawProperty(SerializedProperty property, float width, float labelWidth, string propertyName, string label)
        {
            currentRect.width = width;
            EditorGUIUtility.labelWidth = labelWidth;
            GUIContent content = GUIContent.none;
            if (!string.IsNullOrEmpty(label)) content = new GUIContent(label);
            EditorGUI.PropertyField(currentRect, property.FindPropertyRelative(propertyName), content);
            currentRect.x += currentRect.width + spaces;
        }
    }
}