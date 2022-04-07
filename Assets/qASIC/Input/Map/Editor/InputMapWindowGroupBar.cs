#if UNITY_EDITOR
using UnityEditor;
using qASIC.EditorTools;
using UnityEngine;

using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapWindowGroupBar : InputMapGroupBar
    {
        const string selectedGroupPrefsKey = "qASIC_input_map_editor_group";

        private int? _selectedGroupIndex;
        public override int SelectedGroupIndex
        {
            get
            {
                if (_selectedGroupIndex == null)
                    _selectedGroupIndex = EditorPrefs.GetInt(selectedGroupPrefsKey, 0);

                return _selectedGroupIndex ?? 0;
            }
            set
            {
                EditorPrefs.SetInt(selectedGroupPrefsKey, value);
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
            menu.AddToggableItem("Move right", false, () => Move(groupIndex, 1), groupIndex < Map.Groups.Count - 1);
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
            InputMapWindow.SetMapDirty();
        }

        public override void SetAsDefault(int index)
        {
            base.SetAsDefault(index);
            InputMapWindow.SetMapDirty();
        }

        public void Move(int index, int amount)
        {
            int newIndex = index + amount;
            int groupCount = Map.Groups.Count;

            if (!Map) return;
            Debug.Assert(index >= 0 && index < Map.Groups.Count, $"Cannot move group {index}, index is out of range!");

            if (newIndex < 0 || newIndex >= groupCount) return;

            InputMapWindow.SetMapDirty();

            InputGroup group = Map.Groups[index];
            Map.Groups.RemoveAt(index);
            Map.Groups.Insert(newIndex, group);

            Select(newIndex);
        }

        public void Duplicate(int index)
        {
            InputMapWindow.SetMapDirty();

            if (!Map) return;
            Debug.Assert(index >= 0 && index < Map.Groups.Count, $"Cannot duplicate group {index}, index is out of range!");

            InputGroup group = JsonUtility.FromJson<InputGroup>(JsonUtility.ToJson(Map.Groups[index]));
            group.groupName = InputMapWindowEditorUtility.GenerateUniqueName(group.groupName, Map.GroupExists);

            Map.Groups.Insert(index + 1, group);

            Select(index + 1);
        }

        public override void Add(int index, InputGroup group)
        {
            base.Add(index, group);
            InputMapWindow.SetMapDirty();
        }
    }
}

#endif