#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace qASIC.Internal
{
    [CustomPropertyDrawer(typeof(ObjectRequiresAttribute))]
    public class ObjectRequiresAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label, "Use [ObjectRequires] with object references.");
                return;
            }

            //position = HelpBoxGUI(position, property);

            EditorGUI.PropertyField(position, property);

            object obj = fieldInfo.GetValue(property.serializedObject.targetObject);

            //idk, but this only works
            //I hate this
            if (obj?.Equals(null) != false)
                return;

            List<Type> remainingTypes = GetRemainingComponentTypes(obj, ((ObjectRequiresAttribute)attribute).RequiredTypes);
            if (remainingTypes.Count != 0)
            {
                fieldInfo.SetValue(property.serializedObject.targetObject, null);
                Debug.LogError($"Property {property.propertyPath} requires {string.Join(", ", remainingTypes)} {(remainingTypes.Count > 1 ? "components" : "component")}");
            }
        }

        List<Type> GetRemainingComponentTypes(object obj, Type[] types)
        {
            List<Type> remainingTypes = new List<Type>();
            switch (obj)
            {
                case GameObject gameObj:
                    foreach (Type type in types)
                        if (gameObj.GetComponent(type) == null)
                            remainingTypes.Add(type);
                    break;
                default:
                    Type objType = obj.GetType();
                    foreach (Type type in types)
                        if (!type.IsAssignableFrom(objType) && objType != type)
                            remainingTypes.Add(type);
                    break;
            }

            return remainingTypes;
        }
    }
}
#endif