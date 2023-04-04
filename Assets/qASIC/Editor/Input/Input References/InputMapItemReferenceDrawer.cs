using UnityEditor;
using UnityEngine;
using qASIC.Input.Map.Internal;
using System;

namespace qASIC.Input.Internal
{
    [CustomPropertyDrawer(typeof(InputMapItemReference))]
    public class InputMapItemReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty guidProperty = property.FindPropertyRelative("guid");
            string guid = guidProperty.stringValue;

            Action<string> onChangeValue = newGuid =>
            {
                guidProperty.stringValue = newGuid;
                guidProperty.serializedObject.ApplyModifiedProperties();
            };

            InputGUIUtility.DrawItemReference(position, label, EditorInputManager.Map, guid, onChangeValue);
        }
    }
}