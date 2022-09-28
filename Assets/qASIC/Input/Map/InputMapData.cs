using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace qASIC.InputManagement.Map
{
    [System.Serializable]
    public class InputMapData
    {
        public InputMapData() { }

        public InputMapData(List<InputGroup> groups)
        {
            this.groups = groups;
        }

        public List<InputGroup> groups;

        public void LoadFromData(InputMapData dataToLoad)
        {
            foreach (var groupToLoad in dataToLoad.groups)
            {
                int groupIndex = groups.IndexOf(groups
                    .Where(x => x.guid == groupToLoad.guid)
                    .FirstOrDefault());

                if (groupIndex == -1)
                    continue;

                foreach (var itemToLoad in groupToLoad.items)
                {
                    int itemIndex = groups[groupIndex].items.IndexOf(groups[groupIndex].items
                    .Where(x => x.guid == itemToLoad.guid)
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
