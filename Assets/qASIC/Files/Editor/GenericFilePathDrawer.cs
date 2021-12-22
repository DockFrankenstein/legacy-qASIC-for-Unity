using UnityEditor;
using UnityEngine;
using System;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUI;
using static UnityEditor.EditorGUI;
using static qASIC.UnityEditor.qGUIUtility;

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
            SerializedProperty specialFolder = property.FindPropertyRelative("specialFolder");

            GUIStyle labelStyle = new GUIStyle("Label")
            {
                padding = new RectOffset((int)Spacing, 0, 0, 0),
            };

            labelStyle.normal.background = BackgroundTexture;

            Rect labelPosition = new Rect(position.x, position.y, position.width, singleLineHeight);
            Rect boxPosition = new Rect(position.x, position.y + singleLineHeight, position.width, position.height - singleLineHeight);

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
            Box(boxPosition, BackgroundTexture);

            specialFolder.intValue = (int)(Environment.SpecialFolder)EnumPopup(folderPosition, GUIContent.none, (Environment.SpecialFolder)specialFolder.intValue);
            filePath.stringValue = TextField(filePathPosition, GUIContent.none, filePath.stringValue);

            Label(previewPosition, $"Example: {GenericFilePath.GenerateFullPath((Environment.SpecialFolder)specialFolder.intValue, filePath.stringValue)}");
        }
    }
}