using UnityEngine;
using UnityEditor;
using System.Linq;
using qASIC.Input.Map.Internal;

namespace qASIC.Input.Internal
{
    [CustomPropertyDrawer(typeof(InputKeyAttribute))]
    public class InputKeyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, "Use Input Key Attribute with strings!");
                return;
            }

            if (GUI.Button(position, property.stringValue.Split('/').LastOrDefault(), EditorStyles.popup))
            {
                ShowMenu(position, property);
            }
        }

        void ShowMenu(Rect rect, SerializedProperty property)
        {
            string rootPath = ((InputKeyAttribute)attribute).RootPath;
            PopupWindow.Show(rect, new InputBindingSearchPopupContent(rootPath, a => Popup_OnApply(property, a), new Vector2(rect.width, 200f)));
        }

        void Popup_OnApply(SerializedProperty property, string text)
        {
            property.stringValue = text;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}