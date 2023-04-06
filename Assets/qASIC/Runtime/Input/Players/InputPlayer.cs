using qASIC.Input.Devices;
using qASIC.Input.Map;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using qASIC.Input.Map.ItemData;
using qASIC.ProjectSettings;

namespace qASIC.Input.Players
{
    public class InputPlayer
    {
        public InputPlayer(string id)
        {
            ID = id;
        }

        public InputPlayer(string id, IInputDevice device) : this(id)
        {
            _devices = new List<IInputDevice>(new IInputDevice[] { device });
        }

        public InputPlayer(string id, IInputDevice device, InputMapData mapData) : this(id, device)
        {
            MapData = mapData;
        }

        public string ID { get; set; }
        public InputMapData MapData { get; set; } = new InputMapData();
        public InputMap Map { get; set; }

        private List<IInputDevice> _devices = new List<IInputDevice>();

        public IInputDevice CurrentDevice =>
            _devices.Count == 0 ? null : _devices[0];

        public List<IInputDevice> CurrentDevices =>
            _devices;


        #region Serialization
        public void Save()
        {
            InputProjectSettings.Instance.serializer.Serialize(MapData.PrepareForSerialization());
        }

        public void Load()
        {
            var data = InputProjectSettings.Instance.serializer.Deserialize<InputMapData.SerializableMapData>();
            MapData.LoadSerialization(data);
        }

        public void ResetData()
        {
            MapData = new InputMapData(Map);
        }
        #endregion

        #region Get Input
        public bool GetInput(string itemName) =>
            GetInput(Map?.DefaultGroupName, itemName);

        public bool GetInputUp(string itemName) =>
            GetInputUp(Map?.DefaultGroupName, itemName);

        public bool GetInputDown(string itemName) =>
            GetInputDown(Map?.DefaultGroupName, itemName);

        public bool GetInput(string groupName, string itemName) =>
            GetInputEvent(groupName, itemName).HasFlag(InputEventType.Pressed);

        public bool GetInputUp(string groupName, string itemName) =>
            GetInputEvent(groupName, itemName).HasFlag(InputEventType.Up);

        public bool GetInputDown(string groupName, string itemName) =>
            GetInputEvent(groupName, itemName).HasFlag(InputEventType.Down);

        public bool GetInputFromGUID(string guid) =>
            GetInputEventFromGUID(guid).HasFlag(InputEventType.Pressed);

        public bool GetInputUpFromGUID(string guid) =>
            GetInputEventFromGUID(guid).HasFlag(InputEventType.Up);

        public bool GetInputDownFromGUID(string guid) =>
            GetInputEventFromGUID(guid).HasFlag(InputEventType.Down);
        #endregion

        #region Get Custom Item Input
        public float GetFloatInput(string itemName) =>
            GetInputValue<float>(Map?.DefaultGroupName, itemName);

        public Vector2 GetVector2Input(string itemName) =>
            GetInputValue<Vector2>(Map?.DefaultGroupName, itemName);

        public Vector3 GetVector3Input(string itemName) =>
            GetInputValue<Vector3>(Map?.DefaultGroupName, itemName);

        public float GetFloatInput(string groupName, string itemName) =>
            GetInputValue<float>(groupName, itemName);

        public Vector2 GetVector2Input(string groupName, string itemName) =>
            GetInputValue<Vector2>(groupName, itemName);

        public Vector3 GetVector3Input(string groupName, string itemName) =>
            GetInputValue<Vector3>(groupName, itemName);

        public float GetFloatInputFromGUID(string guid) =>
            GetInputValueFromGUID<float>(guid);

        public Vector2 GetVector2InputFromGUID(string guid) =>
            GetInputValueFromGUID<Vector2>(guid);

        public Vector3 GetVector3InputFromGUID(string guid) =>
            GetInputValueFromGUID<Vector3>(guid);
        #endregion

        #region Get Input Value
        /// <returns>Returns the unclamped value of an item</returns>
        public object GetInputValue(string groupName, string itemName) =>
            InputMapUtility.TryGetItemFromPath(Map, groupName, itemName, out InputMapItem item) ?
            GetInputValueFromItem(item) :
            null;

        public object GetInputValueFromGUID(string guid) =>
            Map != null && Map.ItemsDictionary.ContainsKey(guid) ?
            GetInputValueFromItem(Map.ItemsDictionary[guid]) :
            null;

        public object GetInputValueFromItem(InputMapItem item)
        {
            if (item == null) return default;

            object value = null;

            foreach (IInputDevice device in _devices)
            {
                object readValue = item.ReadValueAsObject(MapData, device.GetInputValue);
                if (value == null)
                {
                    value = readValue;
                    continue;
                }

                value = item.GetHighestValueAsObject(value, readValue);
            }

            return value;
        }

        /// <returns>Returns the unclamped value of an item</returns>
        public T GetInputValue<T>(string groupName, string itemName) =>
            InputMapUtility.TryGetItemFromPath(Map, groupName, itemName, out InputMapItem<T> item) ?
            GetInputValueFromItem(item) :
            default;

        public T GetInputValueFromGUID<T>(string guid) =>
            Map != null && Map.ItemsDictionary.ContainsKey(guid) ?
            GetInputValueFromItem(Map.ItemsDictionary[guid] as InputMapItem<T>) :
            default;

        public T GetInputValueFromItem<T>(InputMapItem<T> item)
        {
            if (item == null) return default;

            T value = default;

            foreach (IInputDevice device in _devices)
            {
                T readValue = item.ReadValue(MapData, device.GetInputValue);
                value = item.GetHighestValue(value, readValue);
            }

            return value;
        }

        public InputEventType GetInputEvent(string itemName) =>
            GetInputEvent(Map?.DefaultGroupName, itemName);

        public InputEventType GetInputEvent(string groupName, string itemName) =>
            InputMapUtility.TryGetItemFromPath(Map, groupName, itemName, out InputMapItem item) ?
            GetInputEventFromItem(item) :
            InputEventType.None;

        public InputEventType GetInputEventFromGUID(string guid) =>
            Map != null && Map.ItemsDictionary.ContainsKey(guid) ?
            GetInputEventFromItem(Map.ItemsDictionary[guid]) :
            InputEventType.None;

        public InputEventType GetInputEventFromItem(InputMapItem item)
        {
            if (item == null) return default;

            InputEventType type = InputEventType.None;

            foreach (IInputDevice device in _devices)
                type |= item.GetInputEvent(MapData, keyPath => device.GetInputEvent(keyPath));

            return type;
        }
        #endregion

        #region Remapping
        public void ChangeInput(string itemName, int index, string key, bool save = true, bool log = true) =>
            ChangeInput(Map?.DefaultGroupName, itemName, index, key, save, log);

        public void ChangeInput(string groupName, string itemName, int index, string key, bool save = true, bool log = true)
        {
            if (!InputMapUtility.TryGetItemFromPath(Map, groupName, itemName, out InputMapItem item))
            {
                qDebug.LogError($"[Input Player] Couldn't remap item {groupName}/{itemName}:{index}, item doesn't exist!");
                return;
            }

            if (!(item is InputBinding))
            {
                qDebug.LogError($"[Input Player] Couldn't remap item {groupName}/{itemName}:{index}, item is not a binding!");
                return;
            }

            var list = MapData.GetItemData<InputBindingData>(item.Guid).keys;

            if (!list.IndexInRange(index))
            {
                qDebug.LogError($"[Input Player] Couldn't remap item {groupName}/{itemName}:{index}, key index is out of range!");
                return;
            }

            list[index] = key;

            if (log)
                qDebug.Log($"[Input Player] Changed key {groupName}/{itemName}:{index} to '{key}'", "input");

            if (save)
                InputManager.SavePreferences();
        }
        #endregion
    }
}