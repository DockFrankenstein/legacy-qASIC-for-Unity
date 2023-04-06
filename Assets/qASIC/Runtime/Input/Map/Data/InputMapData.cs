using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace qASIC.Input.Map
{
    [Serializable]
    public class InputMapData : IEnumerable<KeyValuePair<string, InputMapItemData>>
    {
        public InputMapData() { }

        public InputMapData(InputMap map)
        {
            foreach (var item in map.ItemsDictionary)
            {
                if (!(item.Value is ISerializableMapItem serializableItem))
                    continue;

                SerializableValues.Add(item.Key, serializableItem.CreateDataHolder());
            }
        }

        private Dictionary<string, InputMapItemData> SerializableValues { get; set; } = new Dictionary<string, InputMapItemData>();

        public IEnumerator<KeyValuePair<string, InputMapItemData>> GetEnumerator() =>
            SerializableValues.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public T GetItemData<T>(string itemGUID) where T : InputMapItemData =>
            (T)GetItemData(itemGUID);

        public InputMapItemData GetItemData(string itemGUID) =>
            SerializableValues.TryGetValue(itemGUID, out var data) ? data : null;

        public void SetItemData(string itemGUID, InputMapItemData data)
        {
            if (!SerializableValues.ContainsKey(itemGUID))
            {
                qDebug.LogError($"Item '{itemGUID}' data does not exist!");
                return;
            }

            if (SerializableValues[itemGUID].GetType() != data.GetType())
            {
                qDebug.LogError($"Item '{itemGUID}' data is of a different type!");
                return;
            }

            SerializableValues[itemGUID] = data;
        }

        public SerializableMapData PrepareForSerialization() =>
            new SerializableMapData()
            {
                items = SerializableValues
                    .Select(x => new SerializableItem(x.Key, x.Value))
                    .ToList()
            };

        public void LoadSerialization(SerializableMapData data)
        {
            if (data == null) return;

            Dictionary<string, InputMapItemData> dataValues = new Dictionary<string, InputMapItemData>();

            foreach (var item in data.items)
            {
                if (dataValues.ContainsKey(item.path))
                {
                    qDebug.LogError($"[Input Map Data] File contained duplicate items with path '{item.path}'");
                    continue;
                }

                dataValues.Add(item.path, item.data);
            }

            List<string> valueKeys = SerializableValues
                .Select(x => x.Key)
                .ToList();

            foreach (var key in valueKeys)
                if (dataValues.ContainsKey(key))
                    SerializableValues[key] = dataValues[key];
        }

        public class SerializableMapData
        {
            public List<SerializableItem> items;
        }

        [Serializable]
        public struct SerializableItem
        {
            public SerializableItem(string path, InputMapItemData data)
            {
                this.path = path;
                this.data = data;
            }

            public string path;
            [SerializeReference] public InputMapItemData data;
        }
    }
}