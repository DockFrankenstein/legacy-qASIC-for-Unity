using UnityEditor;
using UnityEngine;
using qASIC.Input.Map.Internal;
using qASIC.Input.Map;
using System;

namespace qASIC.Input.Internal
{
    [CustomPropertyDrawer(typeof(InputMapItemReference))]
    public class InputMapItemReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
            DrawItemReferenceProperty(position, property, label);

        /// <summary>Draws item reference from property</summary>
        public static void DrawItemReferenceProperty(Rect position, SerializedProperty property, GUIContent label) =>
            DrawItemReferenceProperty(position, property, label, typeof(InputMapItem));

        /// <summary>Draws item reference from property</summary>
        /// <param name="itemType">Type of the target Input Map Item</param>
        public static void DrawItemReferenceProperty(Rect position, SerializedProperty property, GUIContent label, Type itemType)
        {
            SerializedProperty guidProperty = property.FindPropertyRelative("guid");
            string guid = guidProperty.stringValue;

            Action<string> onChangeValue = newGuid =>
            {
                guidProperty.stringValue = newGuid;
                guidProperty.serializedObject.ApplyModifiedProperties();
            };

            //Remove prefix if it exists
            if (label.text.Split('_').Length == 2)
            {
                label.text = label.text.Split('_')[1];

                if (label.text.Length > 1)
                    label.text = $"{label.text[0].ToString().ToUpper()}{label.text.Substring(1, label.text.Length - 1)}";
            }

            InputGUIUtility.DrawItemReference(position, label, EditorInputManager.Map, guid, onChangeValue, itemType);
        }
    }
}