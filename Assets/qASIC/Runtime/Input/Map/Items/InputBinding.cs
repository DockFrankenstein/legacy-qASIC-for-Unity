using qASIC.Input.Devices;
using qASIC.Input.Internal.KeyProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace qASIC.Input.Map
{
    [Serializable]
    public class InputBinding : InputMapItem<float>
    {
        public InputBinding() : base() { }
        public InputBinding(string name) : base(name) { }

        public const string DEFAULT_KEY_LIST_NAME = "";

        public List<string> keys = new List<string>();

        public override float ReadValue(Func<string, float> func)
        {
            float value = 0f;
            foreach (string key in keys)
            {
                float keyValue = func(key);
                if (keyValue > value)
                    value = keyValue;
            }

            return value;
        }

        public override InputEventType GetInputEvent(Func<string, InputEventType> func)
        {
            InputEventType type = InputEventType.None;
            foreach (string key in keys)
                type |= func.Invoke(key);

            return type;
        }

        public override float GetHighestValue(float a, float b) =>
            a > b ? a : b;

        public List<KeyList> GetSortedKeys()
        {
            Dictionary<string, KeyList> sortedKeys = InputMapUtility.KeyTypeProviders
                .Select(x => new KeyList(x))
                .ToDictionary(x => x.keyName);

            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                string[] sortedPath = key.Split('/');
                string path = sortedPath[0];

                if (!sortedKeys.ContainsKey(path))
                {
                    if (!sortedKeys.ContainsKey(DEFAULT_KEY_LIST_NAME))
                        sortedKeys.Add(DEFAULT_KEY_LIST_NAME, new KeyList(DEFAULT_KEY_LIST_NAME));

                    path = DEFAULT_KEY_LIST_NAME;
                }

                sortedKeys[path].keys.Add(new KeyList.Key(key, i));
            }

            return sortedKeys
                .Select(x => x.Value)
                .ToList();
        }

        public void SetKeysFromSortedList(List<KeyList> list)
        {
            var listKeys = list.SelectMany(x => x.keys);
            int count = listKeys.Count();
            keys = new List<string>(new string[count]);
            listKeys.ToList().ForEach(x => keys[x.index] = x.path);
        }

        public struct KeyList
        {
            public KeyList(string keyName)
            {
                this.keyName = keyName;
                keys = new List<Key>();
            }

            public KeyList(KeyTypeProvider provider) : this(provider.KeyName) { }

            public string keyName;
            public List<Key> keys;

            public struct Key
            {
                public Key(string path, int index)
                {
                    this.path = path;
                    this.index = index;
                }

                public string path;
                public int index;
            }
        }
    }
}