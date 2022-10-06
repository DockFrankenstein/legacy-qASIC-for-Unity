#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using static UnityEngine.GUI;
using static UnityEditor.EditorGUI;
using static UnityEditor.EditorGUIUtility;

namespace qASIC.Internal
{
    [CustomPropertyDrawer(typeof(KeyCodeListenerAttribute))]
    public class KeyCodeListenerAttributeDrawer : PropertyDrawer
    {
        bool isListening;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = singleLineHeight;
            Rect basePosition = position;

            if (property.propertyType != SerializedPropertyType.Enum)
            {
                LabelField(position, label.text, "Use KeyCodeListener with KeyCode.");
                return;
            }

            switch (isListening)
            {
                case true:
                    position = PrefixLabel(position, label);
                    if (Button(position, "Cancel"))
                        isListening = false;
                    break;
                default:
                    position.width -= 60f;
                    property.intValue = (int)(KeyCode)EnumPopup(position, label, (KeyCode)property.intValue);
                    position.x += position.width;
                    position.width = basePosition.width - position.width;

                    if (Button(position, "Change"))
                        isListening = true;
                    break;
            }

            KeyCode key = (KeyCode)property.intValue;
            ListenForKey(ref key);
            property.intValue = (int)key;
        }

        void ListenForKey(ref KeyCode key)
        {
            if (!isListening) return;

            Event e = Event.current;
            if (e.type != EventType.KeyDown) return;

            key = e.keyCode;
            isListening = false;
        }
    }
}
#endif