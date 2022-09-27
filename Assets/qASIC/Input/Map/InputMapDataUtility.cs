using System.Linq;

namespace qASIC.InputManagement.Map
{
    public static class InputMapDataUtility
    {
        public static bool CompareNames(string a, string b) =>
    a.ToLower() == b.ToLower();

        public static InputMapItem GetItem(InputMapData mapData, string groupName, string actionName) =>
            TryGetItem(mapData, groupName, actionName, out InputMapItem action) ? action : null;

        public static bool TryGetItem(InputMapData mapData, string groupName, string actionName, out InputMapItem item)
        {
            item = null;

            if (!TryGetGroup(mapData, groupName, out InputGroup group))
                return false;

            InputMapItem[] items = group.items
                .Where(x => CompareNames(x.itemName, actionName))
                .ToArray();

            if (items.Length != 1)
                return false;

            item = items[0];
            return true;
        }

        public static bool TryGetGroup(InputMapData mapData, string groupName, out InputGroup group)
        {
            group = null;
            InputGroup[] groups = mapData.groups
                .Where(x => CompareNames(x.groupName, groupName))
                .ToArray();

            if (groups.Length != 1)
                return false;

            group = groups[0];
            return true;
        }
    }
}