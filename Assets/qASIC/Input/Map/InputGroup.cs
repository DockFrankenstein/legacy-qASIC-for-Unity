using System;
using System.Collections.Generic;
using qASIC.Tools;
using UnityEngine;

namespace qASIC.InputManagement.Map
{
    [Serializable]
    public class InputGroup : INonRepeatable
    {
        public InputGroup() { }

        public InputGroup(string name)
        {
            groupName = name;
        }

        public string groupName;
        public string guid = Guid.NewGuid().ToString();

        [SerializeReference] public List<InputMapItem> items = new List<InputMapItem>();

        public string ItemName { get => groupName; set => groupName = value; }

        public override string ToString() =>
            groupName;

        public bool TryGetItem(string itemName, out InputMapItem item, bool logError = false)
        {
            bool contains = NonRepeatableChecker.TryGetItem(items, itemName, out item);

            if (!contains && logError)
                qDebug.LogError($"Group '{groupName}' does not contain action '{itemName}'");

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
            groupName.ToLower() == name.ToLower();

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