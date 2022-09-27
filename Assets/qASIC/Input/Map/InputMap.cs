using UnityEngine;
using System.Collections.Generic;
using qASIC.Tools;
using System.Linq;

namespace qASIC.InputManagement.Map
{
    [CreateAssetMenu(fileName = "NewInputMap", menuName = "qASIC/Input/Input Map")]
    public class InputMap : ScriptableObject
    {
        public int defaultGroup = 0;
        public List<InputGroup> groups = new List<InputGroup>(new InputGroup[] { new InputGroup("Default") });

        [System.NonSerialized] private Dictionary<string, InputGroup> _groupsDictionary = null;
        public Dictionary<string, InputGroup> GroupsDictionary
        {
            get
            {
                if (_groupsDictionary == null)
                {
                    _groupsDictionary = groups
                        .ToDictionary(x => x.guid);
                }

                return _groupsDictionary;
            }
        }

        [System.NonSerialized] Dictionary<string, InputMapItem> _itemsDictionary = null;
        public Dictionary<string, InputMapItem> ItemsDictionary
        {
            get
            {
                if (_itemsDictionary == null)
                {
                    _itemsDictionary = groups
                        .SelectMany(x => x.items)
                        .ToDictionary(x => x.guid);
                }

                return _itemsDictionary;
            }
        }

        public T GetItem<T>(string guid) where T : InputMapItem
        {
            if (ItemsDictionary.ContainsKey(guid))
                return null;

            return (T)ItemsDictionary[guid];
        }

        public InputMapData GetData()
        {
            InputMapData data = new InputMapData(defaultGroup, groups);
            data = data.Duplicate();
            return data;
        }

        public string DefaultGroupName
        { 
            get
            {
                if (defaultGroup >= 0 && defaultGroup < groups.Count)
                    return groups[defaultGroup].groupName;
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

        /// <summary>Checks if there are no duplicate groups</summary>
        public void CheckForRepeating() =>
            NonRepeatableChecker.ContainsRepeatable(groups);

        public string[] GetGroupNames() =>
            groups
            .Select(x => x.groupName)
            .ToArray();

        public bool GroupExists(string groupName) =>
            groups
            .Select(x => x.NameEquals(groupName))
            .Contains(true);

        public bool CanRenameGroup(string newName) =>
            !string.IsNullOrWhiteSpace(newName) && !GroupExists(newName);
    }
}