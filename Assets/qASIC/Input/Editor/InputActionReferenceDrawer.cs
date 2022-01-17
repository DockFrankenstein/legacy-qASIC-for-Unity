using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;

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

            groupProperty.stringValue = EditorGUI.TextField(groupRect, groupProperty.stringValue);
            actionProperty.stringValue = EditorGUI.TextField(actionRect, actionProperty.stringValue);

            if (GUI.Button(buttonRect, "Change"))
            {
                InputReferenceExplorerWindow.OpenProperty(property);
            }
        }
    }
}