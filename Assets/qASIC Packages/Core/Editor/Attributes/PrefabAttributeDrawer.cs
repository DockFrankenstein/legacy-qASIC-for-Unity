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
                EditorGUI.LabelField(position, "Use [Prefab] with object references.");
                return;
            }

            PrefabAttribute attr = (PrefabAttribute)attribute;

            object obj = GetObject(property);

            //idk, but this only works
            //I hate this
            if (obj?.Equals(null) != false)
            {
                EditorGUI.PropertyField(position, property);
                return;
            }

            GameObject gameObj;

            switch (GetObject(property))
            {
                case GameObject go:
                    gameObj = go;
                    break;
                case MonoBehaviour behaviour:
                    gameObj = behaviour.gameObject;
                    break;
                default:
                    EditorGUI.LabelField(position, "Use [Prefab] with GameObject or MonoBehaviour.");
                    return;
            }

            EditorGUI.PropertyField(position, property);

            if (PrefabUtility.GetPrefabAssetType(gameObj) == PrefabAssetType.NotAPrefab)
            {
                fieldInfo.SetValue(property.serializedObject.targetObject, null);
                Debug.LogError($"Property {property.propertyPath} needs to be a prefab!");
                return;
            }

            if (attr.canBeInScene)
                return;

            if (gameObj.scene.name != null)
            {
                fieldInfo.SetValue(property.serializedObject.targetObject, null);
                Debug.LogError($"Property {property.propertyPath} needs to be a prefab!");
            }
        }

        object GetObject(SerializedProperty property) =>
            fieldInfo.GetValue(property.serializedObject.targetObject);
    }
}
#endif