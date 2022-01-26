﻿using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUI;
using static UnityEditor.EditorGUI;

namespace qASIC.FileManagement.Internal
{
    [CustomPropertyDrawer(typeof(GenericFilePath))]
    public class GenericFilePathDrawer : PropertyDrawer
    {
        float Spacing { get => 8f; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            singleLineHeight * 3f + standardVerticalSpacing * 2f + Spacing;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty filePath = property.FindPropertyRelative("filePath");
            SerializedProperty genericFolder = property.FindPropertyRelative("genericFolder");

            GUIStyle labelStyle = new GUIStyle("Tab onlyOne")
            {
                padding = new RectOffset((int)Spacing, 0, 0, 0),
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
            };

            Rect labelPosition = new Rect(position.x, position.y, position.width, singleLineHeight);
            Rect boxPosition = new Rect(position).MoveTop(singleLineHeight);

            position.y += singleLineHeight + standardVerticalSpacing;

            position.height = singleLineHeight;
            position.width -= Spacing;
            position.x += Spacing / 2f;
            position.y += Spacing / 2f;

            Rect folderPosition = new Rect(position.x, position.y, 180f, singleLineHeight);
            Rect filePathPosition = new Rect(position.x + folderPosition.width + standardVerticalSpacing, position.y, position.width - folderPosition.width - standardVerticalSpacing, singleLineHeight);

            position.y += singleLineHeight + standardVerticalSpacing;

            Rect previewPosition = position;

            Label(labelPosition, label, labelStyle);
            Box(boxPosition, GUIContent.none, Styles.Background);

            genericFolder.intValue = (int)(GenericFolder)EnumPopup(folderPosition, GUIContent.none, (GenericFolder)genericFolder.intValue);
            filePath.stringValue = TextField(filePathPosition, GUIContent.none, filePath.stringValue);

            Label(previewPosition, $"Example: {GenericFilePath.GenerateFullPath((GenericFolder)genericFolder.intValue, filePath.stringValue)}");
        }

        class Styles
        {
            public static GUIStyle Background => new GUIStyle("HelpBox");
        }
    }
}