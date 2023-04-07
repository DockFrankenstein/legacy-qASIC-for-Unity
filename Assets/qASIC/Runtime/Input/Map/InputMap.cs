using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using qASIC.Input.Serialization;
using qASIC;

namespace qASIC.Input.Map
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewInputMap", menuName = "qASIC/Input/Input Map")]
    public class InputMap : ScriptableObject
    {
        public int defaultGroup = 0;
        public List<InputGroup> groups = new List<InputGroup>(new InputGroup[] { new InputGroup("Default") });

        [System.NonSerialized] bool _initialized = false;
        public bool Initialized => _initialized;

        [System.NonSerialized] private Dictionary<string, InputGroup> _groupsDictionary = null;
        public Dictionary<string, InputGroup> GroupsDictionary
        {
            get
            {
                if (_groupsDictionary == null)
                    RebuildItemCache();

                return _groupsDictionary;
            }
        }

        [System.NonSerialized] Dictionary<string, InputMapItem> _itemsDictionary = null;
        public Dictionary<string, InputMapItem> ItemsDictionary
        {
            get
            {
                if (_itemsDictionary == null)
                    RebuildItemCache();

                return _itemsDictionary;
            }
        }

        /// <summary>Initializes the map. Call this before using the map.</summary>
        public void Initialize()
        {
            RebuildItemCache();
            ItemsDictionary.ForEach(x => x.Value.map = this);
            GroupsDictionary.ForEach(x => x.Value.map = this);

            _initialized = true;
            qDebug.LogInternal($"[Cablebox] Initialized Input Map '{name}:{GetInstanceID()}'", "input");
        }

        /// <summary>Rebuilds items and groups dictionary</summary>
        public void RebuildItemCache()
        {
            _itemsDictionary = groups
                        .SelectMany(x => x.items)
                        .Where(x => x != null)
                        .ToDictionary(x => x.Guid);

            _groupsDictionary = groups
                .Where(x => x != null)
                .ToDictionary(x => x.Guid);
        }


        public void AddItem(InputGroup group) =>
            InsertItem(groups.Count, group);

        public void InsertItem(int index, InputGroup group)
        {
            group.map = this;
            groups.Insert(index, group);
            RebuildItemCache();
        }

        public void RemoveItem(InputGroup group) =>
            RemoveItem(groups.IndexOf(group));

        public void RemoveItem(int index)
        {
            if (!groups.IndexInRange(index))
                throw new System.IndexOutOfRangeException("Couldn't remove item");

            groups.RemoveAt(index);
            RebuildItemCache();
        }

        ///<summary>Looks for an item of the specified guid from the items cache.</summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <returns>The specified item</returns>
        public T GetItem<T>(string guid) where T : InputMapItem
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            if (!ItemsDictionary.ContainsKey(guid))
                return null;

            return (T)ItemsDictionary[guid];
        }
         
        public string DefaultGroupName
        { 
            get
            {
                if (defaultGroup >= 0 && defaultGroup < groups.Count)
                    return groups[defaultGroup].ItemName;

                return string.Empty;
            }
        }

        public bool TryGetGroup(string groupName, out InputGroup group, bool logError = false)
        {
            bool contains = NonRepeatableChecker.TryGetItem(groups, groupName, out group);

            if (!contains && logError)
                qDebug.LogError($"Map does not contain group '{groupName}'");

            return contains;
        }

        public InputGroup GetGroup(string groupName)
        {
            TryGetGroup(groupName, out InputGroup group, true);
            return group;
        }

        public string[] GetGroupNames() =>
            groups
            .Select(x => x.ItemName)
            .ToArray();

        public bool GroupExists(string groupName) =>
            groups
            .Select(x => x.NameEquals(groupName))
            .Contains(true);
    }
}