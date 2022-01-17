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
        float LeftSpacing => 8f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            singleLineHeight * 3f + standardVerticalSpacing * 3f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Properties
            SerializedProperty groupProperty = property.FindPropertyRelative("groupName");
            SerializedProperty useDefaultProperty = property.FindPropertyRelative("useDefaultGroup");
            SerializedProperty actionProperty = property.FindPropertyRelative("actionName");

            //Rects
            Rect baseRect = new Rect(position);

            position = position.BorderRect(standardVerticalSpacing + LeftSpacing, standardVerticalSpacing, 0f, 0f);
            position.height = singleLineHeight;

            Rect backgroundRect = new Rect(baseRect.x, baseRect.y + singleLineHeight, baseRect.width, baseRect.height - singleLineHeight);
            Rect labelRect = new Rect(baseRect.position, new Vector2(baseRect.width, singleLineHeight));

            position.y += singleLineHeight + standardVerticalSpacing;

            Rect groupLabelRect = new Rect(position.x, position.y, (position.width - ButtonWidth) / 2, position.height);
            Rect actionLabelRect = new Rect(position.x + (position.width - ButtonWidth) / 2 + standardVerticalSpacing, position.y, (position.width - ButtonWidth) / 2 - standardVerticalSpacing * 2, position.height);

            Rect groupRect = new Rect(groupLabelRect);
            Rect actionRect = new Rect(actionLabelRect);
            Rect buttonRect = new Rect(position.x + position.width - ButtonWidth, position.y + singleLineHeight, ButtonWidth, position.height);

            groupRect.y += singleLineHeight;
            actionRect.y += singleLineHeight;

            //Style
            GUIStyle labelStyle = new GUIStyle("Label")
            {
                padding = new RectOffset((int)LeftSpacing, 0, 0, 0),
            };
            labelStyle.normal.background = qGUIUtility.qASICBackgroundTexture;

            //Drawing
            GUI.Box(backgroundRect, GUIContent.none);

            GUI.Label(labelRect, label, labelStyle);

            GUI.Label(groupLabelRect, "Group");
            GUI.Label(actionLabelRect, "Action");

            DrawGroupProperty(groupRect, groupProperty, useDefaultProperty.boolValue);

            actionProperty.stringValue = EditorGUI.TextField(actionRect, actionProperty.stringValue);

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
        }
    }
}