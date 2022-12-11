using UnityEngine;
using qASIC.Input.Map;
using System.Linq;

namespace qASIC.Input
{
    [System.Serializable]
    public class InputMapItemReference
    {
        [SerializeField] string guid;

        public string Guid => guid;

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
            if (!InputManager.MapLoaded)
                return null;

            var targets = InputManager.Map.groups
            .Where(x => x.items.Contains(item));

            return targets.Count() == 1 ? targets.First() : null;
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