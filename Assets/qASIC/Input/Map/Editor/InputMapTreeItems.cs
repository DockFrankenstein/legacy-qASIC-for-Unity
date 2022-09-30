#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using qASIC.EditorTools;

namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapContentItemBase : TreeViewItem
    {
        public virtual Texture GetIcon(InputGroup group) => null;
        public virtual string GetTooltip(InputGroup group) => string.Empty;
        public virtual Color BarColor { get; set; } = Color.clear;
        public virtual bool CanDrag { get => false; }


        public InputMapContentItemBase() : base() { }
        public InputMapContentItemBase(int id) : base(id) { }
        public InputMapContentItemBase(int id, int depth) : base(id, depth) { }
    }

    public class InputMapContentMapItem : InputMapContentItemBase
    {
        public string Guid { get; set; }
        public InputMapItem Item { get; set; }

        public InputMapContentMapItem() : base()
        {
            Guid = System.Guid.NewGuid().ToString();
            id = Guid.GetHashCode();
        }

        public InputMapContentMapItem(InputMapItem item) : base(item.Guid.GetHashCode())
        {
            Item = item;
            Guid = item.Guid;
            displayName = item?.ItemName ?? string.Empty;
        }
    }
}
#endif