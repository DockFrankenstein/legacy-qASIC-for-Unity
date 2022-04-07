#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools;

using static UnityEditor.EditorGUIUtility;

namespace qASIC.Tools.Internal
{
    [CustomPropertyDrawer(typeof(TextTreeStyle))]
    public class TextTreeStyleDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            singleLineHeight * 4 + standardVerticalSpacing * 6;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty presetProperty = property.FindPropertyRelative("preset");
            SerializedProperty middleProperty = property.FindPropertyRelative("middle");
            SerializedProperty lastProperty = property.FindPropertyRelative("last");
            SerializedProperty spaceProperty = property.FindPropertyRelative("space");
            SerializedProperty verticalProperty = property.FindPropertyRelative("vertical");

            GUIStyle borderStyle = new GUIStyle();
            borderStyle.normal.background = qGUIEditorUtility.BorderTexture;

            Rect backgroundRect = new Rect(position);

            position.height = singleLineHeight;
            position.y += standardVerticalSpacing;
            position = position.Border(standardVerticalSpacing, 0f);

            Rect labelRect = new Rect(position);
            Rect labelLineRect = new Rect(position)
                .MoveY(singleLineHeight)
                .SetHeight(1f);

            position.y += singleLineHeight + standardVerticalSpacing;

            Rect presetRect = new Rect(position);

            position.y += singleLineHeight + standardVerticalSpacing;

            float fieldWidth = (position.width - standardVerticalSpacing) / 4f;

            Rect middleTextRect = new Rect(position).SetWidth(fieldWidth).MoveRight(standardVerticalSpacing);
            Rect lastTextRect = new Rect(middleTextRect).MoveX(fieldWidth);
            Rect spaceTextRect = new Rect(lastTextRect).MoveX(fieldWidth);
            Rect verticalTextRect = new Rect(spaceTextRect).MoveX(fieldWidth);

            position.y += singleLineHeight + standardVerticalSpacing;

            Rect middleRect = new Rect(middleTextRect).MoveY(singleLineHeight + standardVerticalSpacing);
            Rect lastRect = new Rect(lastTextRect).MoveY(singleLineHeight + standardVerticalSpacing);
            Rect spaceRect = new Rect(spaceTextRect).MoveY(singleLineHeight + standardVerticalSpacing);
            Rect verticalRect = new Rect(verticalTextRect).MoveY(singleLineHeight + standardVerticalSpacing);

            GUI.Box(backgroundRect, GUIContent.none, EditorStyles.helpBox);
            GUI.Label(labelRect, label);
            GUI.Box(labelLineRect, GUIContent.none, borderStyle);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(presetRect, presetProperty);
            if (EditorGUI.EndChangeCheck())
            {
                //This is so stupid
                //I can't just cast a target object because
                //property drawers use serialized object
                //so instead I have to do this terribleness
                TextTreeStyle.Preset preset = (TextTreeStyle.Preset)presetProperty.intValue;
                if (preset != TextTreeStyle.Preset.custom)
                {
                    TextTreeStyle.ModifyPreset(preset, out string middle, out string last, out string space, out string vertical);

                    middleProperty.stringValue = middle;
                    lastProperty.stringValue = last;
                    spaceProperty.stringValue = space;
                    verticalProperty.stringValue = vertical;
                }
            }

            GUI.Label(middleTextRect, "Middle");
            GUI.Label(lastTextRect, "Last");
            GUI.Label(spaceTextRect, "Space");
            GUI.Label(verticalTextRect, "Vertical");

            EditorGUI.BeginDisabledGroup(presetProperty.intValue != 0);

            EditorGUI.PropertyField(middleRect, middleProperty, GUIContent.none);
            EditorGUI.PropertyField(lastRect, lastProperty, GUIContent.none);
            EditorGUI.PropertyField(spaceRect, spaceProperty, GUIContent.none);
            EditorGUI.PropertyField(verticalRect, verticalProperty, GUIContent.none);

            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif