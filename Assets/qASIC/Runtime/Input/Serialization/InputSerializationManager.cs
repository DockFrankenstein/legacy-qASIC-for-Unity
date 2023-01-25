using UnityEngine;
using System.Collections.Generic;
using System;
using qASIC.Tools;
using qASIC.Input.Map;
using System.Reflection;

namespace qASIC.Input.Serialization
{
    public static class InputSerializationManager
    {
        public static Dictionary<Type, ItemSerialziationData> ItemData { get; private set; } = new Dictionary<Type, ItemSerialziationData>();

        internal static void Initialize()
        {
            //Cache serializable map values in types
            var itemTypes = TypeFinder.FindAllTypesList<InputMapItem>();

            foreach (var item in itemTypes)
            {
                if (item == null || item.IsAbstract) return;
                var fields = TypeFinder.FindAllFieldAttributesInClassList(item, typeof(SerializableMapValue), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var dictionary = new Dictionary<string, FieldInfo>();

                foreach (var field in fields)
                {
                    SerializableMapValue attr = field.GetCustomAttribute<SerializableMapValue>();
                    if (dictionary.ContainsKey(attr.Name))
                        throw new AmbiguousMatchException($"Input Map Item of type '{item}' has multiple serializable map values of the same name!");

                    //if (!field.FieldType.IsAssignableFrom(typeof(ICloneable)))
                    //{
                    //    Debug.Log(field.FieldType);
                    //    qDebug.LogError($"Input Map Item of type '{item}' contains a serializable map value ({attr.Name}) on field '{field.Name}' that does not implement the IClonable interface");
                    //    continue;
                    //}

                    dictionary.Add(attr.Name, field);
                }

                var data = new ItemSerialziationData()
                {
                    itemType = item,
                    fields = dictionary,
                };

                ItemData.Add(item, data);
            }
        }

        public class ItemSerialziationData
        {
            public Type itemType;
            public Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
        }
    }
}
