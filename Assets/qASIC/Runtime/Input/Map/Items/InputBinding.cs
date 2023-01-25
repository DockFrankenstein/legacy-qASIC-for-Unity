using System;
using System.Collections.Generic;
using System.Linq;
using qASIC.Input.Serialization;

namespace qASIC.Input.Map
{
    [Serializable]
    public class InputBinding : InputMapItem<float>
    {
        public InputBinding() : base() { }
        public InputBinding(string name) : base(name) { }

        public const string KEYS_SERIALIZABLE_MAP_VALUE_NAME = "keys";

        [SerializableMapValue(KEYS_SERIALIZABLE_MAP_VALUE_NAME)] public List<string> keys = new List<string>();

        public override void OnCreated()
        {

        }

        public override float ReadValue(InputMapData data, Func<string, float> func)
        {
            var keys = data.GetValue<List<string>>(this, KEYS_SERIALIZABLE_MAP_VALUE_NAME);
            float value = 0f;

            foreach (string key in keys)
            {
                float keyValue = func(key);
                if (keyValue > value)
                    value = keyValue;
            }

            return value;
        }

        public override InputEventType GetInputEvent(InputMapData data, Func<string, InputEventType> func)
        {
            var keys = data.GetValue<List<string>>(this, KEYS_SERIALIZABLE_MAP_VALUE_NAME);

            InputEventType type = InputEventType.None;
            foreach (string key in keys)
                type |= func.Invoke(key);

            return type;
        }

        public override float GetHighestValue(float a, float b) =>
            a > b ? a : b;

        public override bool HasErrors() =>
            HasUnassignedPaths().Count != 0;

        /// <summary>Checks if there are any unassigned paths in the binding</summary>
        /// <returns>A list of all unassigned item indexes</returns>
        public List<int> HasUnassignedPaths() =>
            keys
            .Select((x, i) => InputMapUtility.GetProviderFromPath(x) == null ? i : -1)
            .Where(x => x != -1)
            .ToList();
    }
}