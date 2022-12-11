using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace qASIC.Input.Map
{
    [System.Serializable]
    public class InputMapData
    {
        public InputMapData() { }

        public InputMapData(List<InputGroup> groups) : this()
        {
            this.groups = groups;
            AssignMapDataToItems();
        }

        public List<InputGroup> groups = new List<InputGroup>();

        [System.NonSerialized] private Dictionary<string, InputGroup> _groupsDictionary = null;
        public Dictionary<string, InputGroup> GroupsDictionary
        {
            get
            {
                if (_groupsDictionary == null)
                {
                    _groupsDictionary = groups
                        .ToDictionary(x => x.Guid);
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
                        .ToDictionary(x => x.Guid);
                }

                return _itemsDictionary;
            }
        }

        public void LoadFromData(InputMapData dataToLoad)
        {
            foreach (var groupToLoad in dataToLoad.groups)
            {
                int groupIndex = groups.IndexOf(groups
                    .Where(x => x.Guid == groupToLoad.Guid)
                    .FirstOrDefault());

                if (groupIndex == -1)
                    continue;

                groupToLoad.mapData = this;

                foreach (var itemToLoad in groupToLoad.items)
                {
                    int itemIndex = groups[groupIndex].items.IndexOf(groups[groupIndex].items
                    .Where(x => x.Guid == itemToLoad.Guid)
                    .FirstOrDefault());

                    itemToLoad.mapData = this;

                    if (itemIndex == -1)
                        continue;

                    groups[groupIndex].items[itemIndex] = itemToLoad;
                }
            }
        }

        public InputMapData Duplicate()
        {
            string json = JsonUtility.ToJson(this);
            var data = JsonUtility.FromJson<InputMapData>(json);
            data.AssignMapDataToItems();
            return data;
        }

        void AssignMapDataToItems()
        {
            foreach (var group in this.groups)
            {
                group.mapData = this;
                foreach (var item in group.items)
                    item.mapData = this;
            }
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
    }
}
