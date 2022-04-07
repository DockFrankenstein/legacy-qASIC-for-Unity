#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace qASIC.Internal
{
    [CustomPropertyDrawer(typeof(InspectorLabelAttribute))]
    class InspectorLabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorLabelAttribute inspectorLabel = (InspectorLabelAttribute)attribute;

            EditorGUI.PropertyField(position, property, new GUIContent(inspectorLabel.Label));
        }
    }
}
#endif