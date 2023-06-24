#if UNITY_EDITOR
using UnityEditor;
using qASIC.EditorTools;
using UnityEngine;

using static UnityEngine.GUILayout;

namespace qASIC.Input.Map.Internal
{
    public class InputMapWindowGroupBar : InputMapGroupBar
    {
        const string SELECTED_GROUP_PREFS_KEY = "qASIC_input_map_editor_group";

        public InputMapWindow window;

        private int? _selectedGroupIndex;
        public override int SelectedGroupIndex
        {
            get
            {
                if (_selectedGroupIndex == null)
                    _selectedGroupIndex = EditorPrefs.GetInt(SELECTED_GROUP_PREFS_KEY, 0);

                if (Map != null)
                    _selectedGroupIndex = Mathf.Clamp(_selectedGroupIndex ?? 0, 0, Map.groups.Count - 1);

                return _selectedGroupIndex ?? 0;
            }
            set
            {
                EditorPrefs.SetInt(SELECTED_GROUP_PREFS_KEY, value);
                _selectedGroupIndex = value;
            }
        }

        public override bool EnableContextMenus => true;

        public override void OpenContextMenu(int groupIndex)
        {
            GenericMenu menu = new GenericMenu();
            int index = groupIndex;
            menu.AddItem("Add", false, () => Add(index));
            menu.AddToggableItem("Set as default", false, () => SetAsDefault(index), Map.defaultGroup != index);
            menu.AddSeparator("");
            menu.AddToggableItem("Move left", false, () => Move(groupIndex, -1), groupIndex > 0);
            menu.AddToggableItem("Move right", false, () => Move(groupIndex, 1), groupIndex < Map.groups.Count - 1);
            menu.AddSeparator("");
            menu.AddItem("Duplicate", false, () => Duplicate(index));
            menu.AddSeparator("");
            menu.AddToggableItem("Delete", false, () => DeleteGroup(index), Map.defaultGroup != groupIndex);
            menu.ShowAsContext();
        }

        protected override void OnListGUI()
        {
            if (Button("+", EditorStyles.toolbarButton, Width(EditorGUIUtility.singleLineHeight)))
                Add();
        }

        public override void DeleteGroup(int index)
        {
            base.DeleteGroup(index);
            window.SetMapDirty();
        }

        public override void SetAsDefault(int index)
        {
            base.SetAsDefault(index);
            window.SetMapDirty();
        }

        public void Move(int index, int amount)
        {
            int newIndex = index + amount;
            int groupCount = Map.groups.Count;

            if (!Map) return;
            Debug.Assert(index >= 0 && index < Map.groups.Count, $"Cannot move group {index}, index is out of range!");

            if (newIndex < 0 || newIndex >= groupCount) return;

            window.SetMapDirty();

            InputGroup group = Map.groups[index];
            Map.groups.RemoveAt(index);
            Map.groups.Insert(newIndex, group);

            Select(newIndex);
        }

        public void Duplicate(int index)
        {
            window.SetMapDirty();

            if (!Map) return;
            Debug.Assert(index >= 0 && index < Map.groups.Count, $"Cannot duplicate group {index}, index is out of range!");

            InputGroup group = JsonUtility.FromJson<InputGroup>(JsonUtility.ToJson(Map.groups[index]));
            group.ItemName = InputMapWindowUtility.GenerateUniqueName(group.ItemName, Map.GroupExists);

            Map.groups.Insert(index + 1, group);

            Select(index + 1);
        }

        public override void Add(int index, InputGroup group)
        {
            base.Add(index, group);
            window.SetMapDirty();
        }
    }
}

#endif