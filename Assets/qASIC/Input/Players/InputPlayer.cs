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
            Mathf.Clamp(GetRawInputValue(groupName, actionName), 0f, 1f);

        public float GetAxisValue(string groupName, string axisName)
        {
            //TODO: REIMPLEMENT
            return 0f;
        }

        /// <returns>Returns the unclamped value of an action</returns>
        private float GetRawInputValue(string groupName, string actionName)
        {
            if (!InputMapDataUtility.TryGetItem(MapData, groupName, actionName, out InputMapItem item))
                return 0f;

            float value = 0f;

            foreach (IInputDevice device in _devices)
            {
                //List<int> keys = item.ReadValueAsObject(device.KeyType);

                //foreach (int key in keys)
                //{
                //    value += device.GetInputValue(key);
                //}
            }

            return value;
        }

        private bool GetInputEvent(KeyEventType type, string groupName, string actionName)
        {
            if (!InputMapDataUtility.TryGetItem(MapData, groupName, actionName, out InputMapItem action))
                return false;

            foreach (IInputDevice device in _devices)
            {
                //List<int> keys = action.GetKeyList(device.KeyType);

                //foreach (int key in keys)
                //{
                //    if (device.GetInputEvent(type, key))
                //        return true;
                //}
            }

            return false;
        }
    }
}