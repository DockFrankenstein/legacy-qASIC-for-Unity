#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace qASIC.Displayer.Tools
{
    [CustomPropertyDrawer(typeof(DisplayerLine))]
    public class DisplayerLineDrawer : PropertyDrawer
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

            Vector2 show = new Vector2(0, 18);
            Vector2 tag = new Vector2(22, 71);
            Vector2 text = new Vector2(101, position.width - 165);
            Vector2 value = new Vector2(position.width - 60, 60);

            DrawLabel(position, "tag", tag);
            DrawLabel(position, "text", text);
            DrawLabel(position, "value", value);
            position.y += position.height;

            DrawProperty(position, property, "show", show);
            DrawProperty(position, property, "tag", tag);
            DrawProperty(position, property, "text", text);
            DrawProperty(position, property, "value", value);

            EditorGUI.indentLevel = indent;
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
#endif