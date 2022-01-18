using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;
using qASIC.InputManagement.Internal.ReferenceExplorers;

using Manager = qASIC.InputManagement.Internal.EditorInputManager;

using static UnityEditor.EditorGUIUtility;

namespace qASIC.InputManagement.Internal
{
    [CustomPropertyDrawer(typeof(InputActionReference))]
    public class InputActionReferenceDrawer : PropertyDrawer
    {
        int ButtonWidth => 60;
        float Spacing => 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            singleLineHeight * 4f + standardVerticalSpacing * 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Properties
            SerializedProperty groupProperty = property.FindPropertyRelative("groupName");
            SerializedProperty useDefaultProperty = property.FindPropertyRelative("useDefaultGroup");
            SerializedProperty actionProperty = property.FindPropertyRelative("actionName");

            //Rects
            Rect baseRect = new Rect(position);

            position = position.Border(standardVerticalSpacing + Spacing, 0f);
            position.height = singleLineHeight + standardVerticalSpacing;

            Rect backgroundRect = new Rect(baseRect.x, baseRect.y + singleLineHeight - 1f, baseRect.width, baseRect.height - singleLineHeight + 1f);
            Rect labelRect = new Rect(baseRect.position, new Vector2(baseRect.width, singleLineHeight));

            position.y += singleLineHeight;

            Rect groupLabelRect = new Rect(position.x, position.y, (position.width - ButtonWidth) / 2, position.height);
            Rect actionLabelRect = new Rect(position.x + (position.width - ButtonWidth) / 2 + standardVerticalSpacing, position.y, (position.width - ButtonWidth) / 2 - standardVerticalSpacing * 2, position.height);

            Rect groupRect = new Rect(groupLabelRect).MoveY(singleLineHeight);
            Rect actionRect = new Rect(actionLabelRect).MoveY(singleLineHeight);
            Rect buttonRect = new Rect(position.x + position.width - ButtonWidth, position.y + singleLineHeight, ButtonWidth, position.height);

            Rect defaultToggleRect = new Rect(groupRect).MoveY(singleLineHeight + standardVerticalSpacing).SetWidth(position.width);

            //Style
            GUIStyle labelStyle = new GUIStyle("Tab onlyOne")
            {
                padding = new RectOffset((int)Spacing * 2, (int)Spacing * 2, 0, 0),
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
            };

            //Drawing
            GUI.Box(backgroundRect, GUIContent.none, Styles.Background);
            GUI.Label(labelRect, label, labelStyle);

            GUI.Label(groupLabelRect, "Group");
            GUI.Label(actionLabelRect, "Action");

            DrawGroupProperty(groupRect, groupProperty, useDefaultProperty.boolValue);
            actionProperty.stringValue = EditorGUI.TextField(actionRect, actionProperty.stringValue);

            useDefaultProperty.boolValue = EditorGUI.ToggleLeft(defaultToggleRect, "Default group", useDefaultProperty.boolValue);

            if (GUI.Button(buttonRect, "Change"))
            {
                InputActionReferenceExplorerWindow.OpenProperty(property);
            }
        }

        void DrawGroupProperty(Rect rect, SerializedProperty property, bool useDefault)
        {
            if (!useDefault)
            {
                property.stringValue = EditorGUI.TextField(rect, property.stringValue);
                return;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(rect, Manager.Map ? Manager.Map.DefaultGroupName : "Default", Styles.DefaultGroupField);
            EditorGUI.EndDisabledGroup();
        }

        class Styles
        {
            public static GUIStyle DefaultGroupField => new GUIStyle(EditorStyles.textField) { fontStyle = FontStyle.Italic };
            public static GUIStyle Background => new GUIStyle("HelpBox");
        }
    }
}