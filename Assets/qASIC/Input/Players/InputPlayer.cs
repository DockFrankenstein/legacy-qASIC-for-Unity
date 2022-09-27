using qASIC.InputManagement.Devices;
using qASIC.InputManagement.Map;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagement.Players
{
    public class InputPlayer
    {
        public InputPlayer(string id)
        {
            ID = id;
        }

        public InputPlayer(string id, IInputDevice device)
        {
            ID = id;
            _devices = new List<IInputDevice>(new IInputDevice[] { device });
        }

        public InputPlayer(string id, IInputDevice device, InputMapData mapData)
        {
            ID = id;
            _devices = new List<IInputDevice>(new IInputDevice[] { device });
            MapData = mapData;
        }

        public string ID { get; set; }
        public InputMapData MapData { get; set; }

        private List<IInputDevice> _devices = new List<IInputDevice>();

        public IInputDevice CurrentDevice =>
            _devices.Count == 0 ? null : _devices[0];

        public List<IInputDevice> CurrentDevices =>
            _devices;


        public bool GetInput(string groupName, string axisName) =>
            Mathf.Abs(GetInputValue(groupName, axisName)) >= 0.5f;

        public bool GetInputUp(string groupName, string actionName) =>
            GetInputEvent(KeyEventType.up, groupName, actionName);

        public bool GetInputDown(string groupName, string actionName) =>
            GetInputEvent(KeyEventType.down, groupName, actionName);

        public float GetInputValue(string groupName, string actionName) =>
            Mathf.Clamp(GetRawInputValue<float>(groupName, actionName), 0f, 1f);

        /// <returns>Returns the unclamped value of an item</returns>
        public object GetRawInputValue(string groupName, string itemName)
        {
            if (!InputMapDataUtility.TryGetItem(MapData, groupName, itemName, out InputMapItem item))
                return default;

            object value = default;

            foreach (IInputDevice device in _devices)
            {
                object readValue = item.ReadValueAsObject(device.GetInputValue);
                value = item.GetHighestValueAsObject(value, readValue);
            }

            return value;
        }

        /// <returns>Returns the unclamped value of an item</returns>
        public T GetRawInputValue<T>(string groupName, string itemName)
        {
            if (!InputMapDataUtility.TryGetItem<T>(MapData, groupName, itemName, out InputMapItem<T> item))
                return default;

            T value = default;

            foreach (IInputDevice device in _devices)
            {
                T readValue = item.ReadValue(device.GetInputValue);
                value = item.GetHighestValue(value, readValue);
            }

            return value;
        }

        private bool GetInputEvent(KeyEventType type, string groupName, string actionName)
        {
            if (!InputMapDataUtility.TryGetItem(MapData, groupName, actionName, out InputMapItem item))
                return false;

            foreach (IInputDevice device in _devices)
                if (item.GetInputEvent(keyPath => device.GetInputEvent(type, keyPath)))
                    return true;

            return false;
        }
    }
}