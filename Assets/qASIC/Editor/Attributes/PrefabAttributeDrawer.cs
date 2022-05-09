#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace qASIC.Internal
{
    [CustomPropertyDrawer(typeof(PrefabAttribute))]
    public class PrefabAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label, "Use [Prefab] with object references.");
                return;
            }

            EditorGUI.PropertyField(position, property);

            object obj = fieldInfo.GetValue(property.serializedObject.targetObject);

            //idk, but this only works
            //I hate this
            if (obj?.Equals(null) != false)
                return;

            if (PrefabUtility.GetPrefabAssetType(obj as Object) == PrefabAssetType.NotAPrefab)
            {
                fieldInfo.SetValue(property.serializedObject.targetObject, null);
                Debug.LogError($"Property {property.propertyPath} needs to be a prefab");
            }

        }
    }
}
#endif