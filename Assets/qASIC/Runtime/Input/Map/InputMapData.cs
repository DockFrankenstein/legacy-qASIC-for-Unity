using System.Collections.Generic;
using qASIC.Input.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using static qASIC.Input.Map.InputMapData.SerializableInputMapData;

namespace qASIC.Input.Map
{
    [Serializable]
    public class InputMapData
    {
        public InputMapData() { }
        public InputMapData(InputMap map)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            foreach (var item in map.ItemsDictionary)
            {
                SerializableValues.Add(item.Key, new Dictionary<string, object>());
                var typeData = InputSerializationManager.ItemData[item.Value.GetType()];
                
                foreach (var field in typeData.fields)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        var value = field.Value.GetValue(item.Value);
                        formatter.Serialize(stream, value);
                        stream.Position = 0;
                        value = formatter.Deserialize(stream);

                        SerializableValues[item.Key].Add(field.Key, value);
                    }
                }
            }
        }

        public Dictionary<string, Dictionary<string, object>> SerializableValues { get; set; } = new Dictionary<string, Dictionary<string, object>>();

        public SerializableInputMapData CreateSerializableData()
        {
            var data = new SerializableInputMapData();

            foreach (var item in SerializableValues)
            {
                var itemValues = new ItemValues();
                itemValues.itemPath = item.Key;

                foreach (var value in item.Value)
                    itemValues.values.Add(new ItemValues.Value(value.Key, value.Value));

                data.values.Add(itemValues);
            }

            return data;
        }

        public void LoadSerializableData(SerializableInputMapData data)
        {
            Dictionary<string, Dictionary<string, object>> dataDictionary = new Dictionary<string, Dictionary<string, object>>();

            foreach (var item in data.values)
            {
                if (dataDictionary.ContainsKey(item.itemPath)) continue;
                dataDictionary.Add(item.itemPath, new Dictionary<string, object>());

                foreach (var value in item.values)
                {
                    if (dataDictionary[item.itemPath].ContainsKey(value.valueName)) continue;
                    dataDictionary[item.itemPath].Add(value.valueName, value.value);
                }
            }

            foreach (var item in SerializableValues)
            {
                if (!dataDictionary.ContainsKey(item.Key)) continue;
                foreach (var value in item.Value)
                {
                    if (!dataDictionary[item.Key].ContainsKey(value.Key)) continue;
                    SerializableValues[item.Key][value.Key] = value.Value;
                }
            }
        }

        public bool ValueExists(InputMapItem item, string name) =>
            item != null && ValueExists(item.Guid, name);

        public bool ValueExists(string guid, string name) =>
            SerializableValues.TryGetValue(guid, out var values) && 
            values.ContainsKey(name);

        public void SetValue(InputMapItem item, string name, object value) =>
            SetValue(item?.Guid, name, value);

        public void SetValue(string guid, string name, object value) =>
            SerializableValues[guid][name] = value;

        public T GetValue<T>(InputMapItem item, string name) =>
            GetValue<T>(item?.Guid, name);

        public object GetValue(InputMapItem item, string name) =>
            GetValue(item?.Guid, name);

        public T GetValue<T>(string guid, string name)
        {
            return (T)SerializableValues[guid][name];
        }

        public object GetValue(string guid, string name) =>
            SerializableValues[guid][name];

        [Serializable]
        public class SerializableInputMapData
        {
            public List<ItemValues> values = new List<ItemValues>();

            [Serializable]
            public class ItemValues
            {
                public string itemPath;
                public List<Value> values = new List<Value>();

                [Serializable]
                public class Value
                {
                    public Value() { }
                    public Value(string valueName, object value) : this()
                    {
                        this.valueName = valueName;
                        this.value = value;
                    }

                    public string valueName;
                    public object value;
                }
            }
        }
    }
}