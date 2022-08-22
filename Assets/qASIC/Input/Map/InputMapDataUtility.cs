using System.Linq;

namespace qASIC.InputManagement.Map
{
    public static class InputMapDataUtility
    {
        public static bool CompareNames(string a, string b) =>
    a.ToLower() == b.ToLower();

        public static InputAction GetAction(InputMapData mapData, string groupName, string actionName) =>
            TryGetAction(mapData, groupName, actionName, out InputAction action) ? action : null;

        public static bool TryGetAction(InputMapData mapData, string groupName, string actionName, out InputAction action)
        {
            action = null;

            if (!TryGetGroup(mapData, groupName, out InputGroup group))
                return false;

            InputAction[] actions = group.actions
                .Where(x => CompareNames(x.actionName, actionName))
                .ToArray();

            if (actions.Length != 1)
                return false;

            action = actions[0];
            return true;
        }

        public static InputAxis GetAxis(InputMapData mapData, string groupName, string axisName) =>
            TryGetAxis(mapData, groupName, axisName, out InputAxis axis) ? axis : null;

        public static bool TryGetAxis(InputMapData mapData, string groupName, string axisName, out InputAxis axis)
        {
            axis = null;

            if (!TryGetGroup(mapData, groupName, out InputGroup group))
                return false;

            InputAxis[] axes = group.axes
                .Where(x => CompareNames(x.axisName, axisName))
                .ToArray();

            if (axes.Length != 1)
                return false;

            axis = axes[0];
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