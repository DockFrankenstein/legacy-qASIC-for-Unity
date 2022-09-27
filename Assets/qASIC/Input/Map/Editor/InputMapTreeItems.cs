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

        public InputMapContentMapItem(InputMapItem item) : base(item.guid.GetHashCode())
        {
            Item = item;
            Guid = item.guid;
            displayName = item?.ItemName ?? string.Empty;
        }
    }

    //public abstract class InputMapContentEditableItemBase : InputMapContentItemBase
    //{
    //    public virtual bool Deletable => true;
    //    public override bool CanDrag => true;

    //    public abstract void Rename(string newName);
    //    public abstract void Delete(InputGroup group);
    //    public abstract bool CompareContent<t>(t item);
    //}

    //public class InputMapContentActionTreeItem : InputMapContentEditableItemBase
    //{
    //    public InputMapContentActionTreeItem(InputBinding binding, int id) : base()
    //    {
    //        Debug.Assert(binding != null, "Binding cannot be null");
    //        Binding = binding;
    //        displayName = Binding.itemName;
    //        this.id = id;
    //        depth = -1;
    //    }

    //    /// <summary>Renames item - you have to check if it's unique first</summary>
    //    public override void Rename(string newName)
    //    {
    //        Binding.itemName = newName;
    //        displayName = newName;
    //    }

    //    public override void Delete(InputGroup group) =>
    //        group.items.Remove(Binding);

    //    public override bool CompareContent<t>(t item) =>
    //        item is InputBinding a && Binding == a;
    //}

    //public class InputMapContentOtherTreeItem : InputMapContentEditableItemBase
    //{
    //    public InputMapItem Item { get; }

    //    //public override Texture GetIcon(InputGroup group) =>
    //    //    HasErrors(group) ? qGUIEditorUtility.ErrorIcon : null;

    //    //public override string GetTooltip(InputGroup group) =>
    //    //    HasErrors(group) ? "Axis contains incorrect action names" : string.Empty;

    //    public InputMapContentOtherTreeItem(InputMapItem item, int id)
    //    {
    //        Debug.Assert(item != null, "Axis cannot be null");
    //        Item = item;
    //        displayName = Item.itemName;
    //        this.id = id;
    //        depth = -1;
    //    }

    //    public override void Rename(string newName)
    //    {
    //        Item.itemName = newName;
    //        displayName = newName;
    //    }

    //    public override void Delete(InputGroup group) =>
    //        group.items.Remove(Item);

    //    public override bool CompareContent<t>(t item) =>
    //        item is Input1DAxis a && Item == a;
    //}

    ////This is just for recognition
    //public abstract class InputMapContentHeaderItemBase : InputMapContentItemBase
    //{
    //    public InputMapContentHeaderItemBase() : base() { }
    //    public InputMapContentHeaderItemBase(int id) : base(id) { }
    //    public InputMapContentHeaderItemBase(int id, int depth) : base(id, depth) { }
    //    public InputMapContentHeaderItemBase(int id, int depth, string displayName) : base(id, depth, displayName) { }
    //    public InputMapContentHeaderItemBase(int id, int depth, string displayName, Color barColor) : base(id, depth, displayName, barColor) { }
    //}

    //public class InputMapTreeActionHeaderItem : InputMapContentHeaderItemBase
    //{
    //    public InputMapTreeActionHeaderItem() : base() { }
    //    public InputMapTreeActionHeaderItem(int id) : base(id) { }
    //    public InputMapTreeActionHeaderItem(int id, int depth) : base(id, depth) { }
    //    public InputMapTreeActionHeaderItem(int id, int depth, string displayName) : base(id, depth, displayName) { }
    //    public InputMapTreeActionHeaderItem(int id, int depth, string displayName, Color barColor) : base(id, depth, displayName, barColor) { }
    //}

    //public class InputMapTreeAxisHeaderItem : InputMapContentHeaderItemBase
    //{
    //    public InputMapTreeAxisHeaderItem() : base() { }
    //    public InputMapTreeAxisHeaderItem(int id) : base(id) { }
    //    public InputMapTreeAxisHeaderItem(int id, int depth) : base(id, depth) { }
    //    public InputMapTreeAxisHeaderItem(int id, int depth, string displayName) : base(id, depth, displayName) { }
    //    public InputMapTreeAxisHeaderItem(int id, int depth, string displayName, Color barColor) : base(id, depth, displayName, barColor) { }
    //}
}
#endif