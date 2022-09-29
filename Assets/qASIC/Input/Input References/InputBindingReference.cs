using UnityEngine;
using qASIC.InputManagement.Map;
using System.Linq;

namespace qASIC.InputManagement
{
    [System.Serializable]
    public class InputMapItemReference
    {
        [SerializeField] string guid;

        public InputMapItemReference() { }

        public InputMapItemReference(string guid)
        {
            this.guid = guid;
        }

        public bool ItemExists() =>
            InputManager.Map?.ItemsDictionary.ContainsKey(guid) ?? false;

        public InputGroup GetGroup()
        {
            var item = GetItem();
            return InputManager.Map?.groups
            .Where(x => x.items.Contains(item))
            .First();
        }

        public InputMapItem GetItem()
        {
            if (!ItemExists())
                return null;

            return InputManager.Map.ItemsDictionary[guid];
        }

        public string GetGroupName() =>
            GetGroup()?.ItemName ?? string.Empty;

        public string GetItemName() =>
            GetItem()?.ItemName ?? string.Empty;
    }
}