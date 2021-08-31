using UnityEditor;
using UnityEngine;

namespace qASIC.Console.Tools
{
    [CustomPropertyDrawer(typeof(GameConsoleColor))]
    public class GameConsoleColorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Vector2 colorName = new Vector2(0, position.width * 0.5f - 2);
            Vector2 color = new Vector2(position.width * 0.5f + 2, position.width * 0.5f);

            DrawLabel(position, "name", colorName);
            DrawLabel(position, "color", color);
            position.y += position.height;

            DrawProperty(position, property, "colorName", colorName);
            DrawProperty(position, property, "color", color);

            EditorGUI.EndProperty();
        }

        void DrawProperty(Rect position, SerializedProperty property, string propertyName, Vector2 localPosition) =>
            EditorGUI.PropertyField(CalculateRect(position, localPosition), property.FindPropertyRelative(propertyName), GUIContent.none);

        void DrawLabel(Rect position, string label, Vector2 localPosition) =>
            EditorGUI.PrefixLabel(CalculateRect(position, localPosition), GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label));

        Rect CalculateRect(Rect position, Vector2 localPosition) =>
            new Rect(position.x + localPosition.x, position.y, localPosition.y, position.height);
    }
}