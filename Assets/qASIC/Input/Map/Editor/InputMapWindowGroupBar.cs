using UnityEditor;
using qASIC.EditorTools;

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
            menu.AddToggableItem("Set as default", false, () => SetAsDefault(index), Map.defaultGroup != index);
            menu.AddItem("Add", false, () => Add(index));
            menu.AddItem("Delete", false, () => DeleteGroup(index));
            menu.ShowAsContext();
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

        public override void Add(int index, InputGroup group)
        {
            base.Add(index, group);
            InputMapWindow.SetMapDirty();
        }
    }
}
