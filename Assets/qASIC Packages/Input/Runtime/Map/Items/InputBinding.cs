using System;
using System.Collections.Generic;
using System.Linq;
using qASIC.Input.Devices;
using qASIC.Input.Map.ItemData;
using UnityEngine;

namespace qASIC.Input.Map
{
    [Serializable]
    public sealed class InputBinding : InputMapItem<float>, ISerializableMapItem
    {
        public InputBinding() : base() { }
        public InputBinding(string name) : base(name) { }

        public override Color ItemColor => qInfo.CableboxColor;

        public Type DataHolderType => typeof(InputBindingData);

        public List<string> keys = new List<string>();

        public InputMapItemData CreateDataHolder()
        {
            return new InputBindingData()
            {
                keys = new List<string>(keys),
            };
        }

        public override float ReadValue(InputMapData data, IInputDevice device)
        {
            var keys = data.GetItemData<InputBindingData>(Guid).keys;
            float value = 0f;

            foreach (string key in keys)
            {
                float keyValue = device.GetInputValue(key);
                if (keyValue > value)
                    value = keyValue;
            }

            return value;
        }

        public override InputEventType GetInputEvent(InputMapData data, IInputDevice device)
        {
            var keys = data.GetItemData<InputBindingData>(Guid).keys;

            InputEventType type = InputEventType.None;
            foreach (string key in keys)
                type |= device.GetInputEvent(key);

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