#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUI;
using static UnityEditor.EditorGUI;

using Property = qASIC.FileManagement.AdvancedGenericFilePath;

namespace qASIC.FileManagement.Internal
{
    [CustomPropertyDrawer(typeof(Property))]
    public class AdvancedGenericFilePathDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            GenericFilePathDrawer.GetHeight() * 2f + singleLineHeight * 3f + standardVerticalSpacing * 2f + Styles.Border * 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty buildPathProperty = property.FindPropertyRelative(nameof(Property.buildPath));
            SerializedProperty separateEditorPathProperty = property.FindPropertyRelative(nameof(Property.separateEditorPath));
            SerializedProperty editorPathProperty = property.FindPropertyRelative(nameof(Property.editorPath));

            Rect labelRect = new Rect(position)
                .SetHeight(singleLineHeight);
            Rect backgroundRect = new Rect(position)
                .MoveTop(singleLineHeight);

            position = position.Border(Styles.Border)
                .MoveY(singleLineHeight + standardVerticalSpacing);

            Rect buildPathRect = new Rect(position).SetHeight(GenericFilePathDrawer.GetHeight());
            Rect separateEditorPathRect = new Rect(buildPathRect)
                .SetHeight(singleLineHeight)
                .MoveY(buildPathRect.height + singleLineHeight + standardVerticalSpacing);
            Rect editorPathRect = new Rect(separateEditorPathRect)
                .SetHeight(GenericFilePathDrawer.GetHeight())
                .MoveY(singleLineHeight + standardVerticalSpacing);

            Box(backgroundRect, GUIContent.none, Styles.Background);
            Label(labelRect, label, Styles.Label);

            PropertyField(buildPathRect, buildPathProperty);
            PropertyField(separateEditorPathRect, separateEditorPathProperty);

            BeginDisabledGroup(!separateEditorPathProperty.boolValue);
            PropertyField(editorPathRect, editorPathProperty);
            EndDisabledGroup();
        }

        static class Styles
        {
            public static int Border => 4;

            public static GUIStyle Label => new GUIStyle("Tab onlyOne")
            {
                padding = new RectOffset(Border * 2, 0, 0, 0),
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
            };

            public static GUIStyle Background => EditorStyles.helpBox;
        }
    }
}

#endif