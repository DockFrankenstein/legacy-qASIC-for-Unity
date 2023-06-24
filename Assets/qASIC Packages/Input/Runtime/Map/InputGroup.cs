using System;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Input.Map
{
    [Serializable]
    public class InputGroup : INonRepeatable, IMapItem
    {
        public InputGroup() { }

        public InputGroup(string name)
        {
            itemName = name;
        }

        [SerializeField] string itemName;
        [SerializeField] string guid = System.Guid.NewGuid().ToString();

        [SerializeReference] public List<InputMapItem> items = new List<InputMapItem>();

        [NonSerialized] internal InputMap map;
        [NonSerialized] internal InputMapData mapData;

        public string ItemName { get => itemName; set => itemName = value; }
        public string Guid { get => guid; set => guid = value; }

        public override string ToString() =>
            itemName;

        public void AddItem(InputMapItem item) =>
            InsertItem(items.Count, item);

        public void InsertItem(int index, InputMapItem item)
        {
            item.map = map;
            items.Insert(index, item);
            map.RebuildItemCache();
        }

        public void RemoveItem(InputMapItem item) =>
            RemoveItem(items.IndexOf(item));

        public void RemoveItem(int index)
        {
            if (!items.IndexInRange(index))
                throw new IndexOutOfRangeException("Couldn't remove item");

            items.RemoveAt(index);
            map.RebuildItemCache();
        }

        public bool TryGetItem(string itemName, out InputMapItem item, bool logError = false)
        {
            bool contains = NonRepeatableChecker.TryGetItem(items, itemName, out item);

            if (!contains && logError)
                qDebug.LogError($"Group '{this.itemName}' does not contain action '{itemName}'");

            return contains;
        }

        public InputMapItem GetItem(string actionName)
        {
            TryGetItem(actionName, out InputMapItem item, true);
            return item;
        }

        public void CheckForRepeating()
        {
            NonRepeatableChecker.ContainsRepeatable(items);
        }

        public bool NameEquals(string name) =>
            itemName.ToLower() == name.ToLower();

        public bool ItemExists(string actionName)
        {
            for (int i = 0; i < items.Count; i++)
                if (items[i]?.NameEquals(actionName) == true)
                    return true;
            return false;
        }

        public bool CanRenameItem(string newName) =>
            !string.IsNullOrWhiteSpace(newName) && !ItemExists(newName);
    }
}