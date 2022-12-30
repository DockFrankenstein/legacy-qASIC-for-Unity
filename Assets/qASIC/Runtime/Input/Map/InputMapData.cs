using System.Collections.Generic;
using qASIC.Input.Serialization;
using System;
using UnityEngine;

namespace qASIC.Input.Map
{
    [Serializable]
    public class InputMapData
    {
        public InputMapData() { }
        public InputMapData(InputMap map)
        {
            foreach (var item in map.ItemsDictionary)
            {
                SerializableValues.Add(item.Key, new Dictionary<string, object>());
                var typeData = InputSerializationManager.ItemData[item.Value.GetType()];
                
                foreach (var field in typeData.fields)
                {
                    var value = field.Value.GetValue(item.Value);
                    SerializableValues[item.Key].Add(field.Key, value);
                }
            }
        }

        public Dictionary<string, Dictionary<string, object>> SerializableValues { get; set; } = new Dictionary<string, Dictionary<string, object>>();

        public SerializableInputMapData CreateSerializableData()
        {
            var data = new SerializableInputMapData();

            //foreach (var item in SerializableValues)
            //{
            //    var itemValue = new SerializableInputMapData.ItemValues();

            //    foreach (var value in item.Value)
            //    {
            //        System.Text.
            //    }

            //    data.values.Add(new Tuple<string, SerializableInputMapData.ItemValues>(item.Key, itemValue));
            //}

            return data;
        }

        public T GetSerializableValue<T>(InputMapItem item, string name) =>
            GetSerializableValue<T>(item.Guid, name);

        public object GetSerializableValue(InputMapItem item, string name) =>
            GetSerializableValue(item.Guid, name);

        public T GetSerializableValue<T>(string guid, string name)
        {
            return (T)SerializableValues[guid][name];
        }

        public object GetSerializableValue(string guid, string name) =>
            SerializableValues[guid][name];

        [Serializable]
        public class SerializableInputMapData
        {
            public List<Tuple<string, ItemValues>> values = new List<Tuple<string, ItemValues>>();

            public class ItemValues
            {
                public List<Tuple<string, string>> values = new List<Tuple<string, string>>();
                public List<Tuple<string, List<string>>> lists = new List<Tuple<string, List<string>>>();
            }
        }
    }
}
