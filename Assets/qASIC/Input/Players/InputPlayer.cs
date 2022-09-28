﻿using qASIC.InputManagement.Devices;
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


        #region Get Input
        public bool GetInput(string itemName) =>
            GetInput(MapData.DefaultGroupName, itemName);

        public bool GetInputUp(string itemName) =>
            GetInputUp(MapData.DefaultGroupName, itemName);

        public bool GetInputDown(string itemName) =>
            GetInputDown(MapData.DefaultGroupName, itemName);

        public bool GetInput(string groupName, string itemName) =>
            GetInputEvent(groupName, itemName).HasFlag(InputEventType.Pressed);

        public bool GetInputUp(string groupName, string itemName) =>
            GetInputEvent(groupName, itemName).HasFlag(InputEventType.Up);

        public bool GetInputDown(string groupName, string itemName) =>
            GetInputEvent(groupName, itemName).HasFlag(InputEventType.Down);
        #endregion

        #region Get Custom Item Input
        public float GetFloatInput(string itemName) =>
            GetInputValue<float>(MapData.DefaultGroupName, itemName);

        public Vector2 GetVector2Input(string itemName) =>
            GetInputValue<Vector2>(MapData.DefaultGroupName, itemName);

        public Vector3 GetVector3Input(string itemName) =>
            GetInputValue<Vector3>(MapData.DefaultGroupName, itemName);

        public float GetFloatInput(string groupName, string itemName) =>
            GetInputValue<float>(groupName, itemName);

        public Vector2 GetVector2Input(string groupName, string itemName) =>
            GetInputValue<Vector2>(groupName, itemName);

        public Vector3 GetVector3Input(string groupName, string itemName) =>
            GetInputValue<Vector3>(groupName, itemName);
        #endregion

        #region Get Input Value
        /// <returns>Returns the unclamped value of an item</returns>
        public object GetInputValue(string groupName, string itemName)
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
        public T GetInputValue<T>(string groupName, string itemName)
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

        public InputEventType GetInputEvent(string itemName) =>
            GetInputEvent(MapData.DefaultGroupName, itemName);

        public InputEventType GetInputEvent(string groupName, string itemName)
        {
            if (!InputMapDataUtility.TryGetItem(MapData, groupName, itemName, out InputMapItem item))
                return InputEventType.None;

            InputEventType type = InputEventType.None;

            foreach (IInputDevice device in _devices)
                type |= item.GetInputEvent(keyPath => device.GetInputEvent(keyPath));

            return type;
        }
        #endregion
    }
}