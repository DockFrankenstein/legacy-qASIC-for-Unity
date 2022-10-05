using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace qASIC.InputManagement.Map
{
    [System.Serializable]
    public class InputMapData
    {
        public InputMapData() { }

        public InputMapData(List<InputGroup> groups) : this()
        {
            this.groups = groups;
        }

        public List<InputGroup> groups;

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

                foreach (var itemToLoad in groupToLoad.items)
                {
                    int itemIndex = groups[groupIndex].items.IndexOf(groups[groupIndex].items
                    .Where(x => x.Guid == itemToLoad.Guid)
                    .FirstOrDefault());

                    if (itemIndex == -1)
                        continue;

                    groups[groupIndex].items[itemIndex] = itemToLoad;
                }
            }
        }

        public InputMapData Duplicate()
        {
            string json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<InputMapData>(json);
        }
    }
}
