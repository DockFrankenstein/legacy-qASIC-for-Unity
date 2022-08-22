using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagement.Map
{
    [System.Serializable]
    public class InputMapData
    {
        public InputMapData() { }

        public InputMapData(int defaultGroup, List<InputGroup> groups)
        {
            this.defaultGroup = defaultGroup;
            this.groups = groups;
        }

        public int defaultGroup;
        public List<InputGroup> groups;

        public InputMapData Duplicate()
        {
            string json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<InputMapData>(json);
        }
    }
}
