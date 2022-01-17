using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace qASIC.InputManagement.Map.Internal
{
    public abstract class InputMapContentItemBase : TreeViewItem
    {
        public Color BarColor { get; set; } = Color.clear;

        public InputMapContentItemBase() : base() { }
        public InputMapContentItemBase(int id) : base(id) { }
        public InputMapContentItemBase(int id, int depth) : base(id, depth) { }
        public InputMapContentItemBase(int id, int depth, string displayName) : base(id, depth, displayName) { }

        public InputMapContentItemBase(int id, int depth, string displayName, Color barColor) : base(id, depth, displayName)
        {
            BarColor = barColor;
        }
    }

    public abstract class InputMapContentEditableItemBase : InputMapContentItemBase
    {
        public virtual bool Deletable => true;

        public abstract void Rename(string newName);
        public abstract void Delete(InputGroup group);
        public abstract bool CompareContent<t>(t item);
    }

    public class InputMapContentActionTreeItem : InputMapContentEditableItemBase
    {
        public InputAction Action { get; }

        public InputMapContentActionTreeItem(InputAction action, int id)
        {
            Debug.Assert(action != null, "Action cannot be null");
            Action = action;
            displayName = Action.actionName;
            this.id = id;
            depth = -1;
        }

        /// <summary>Renames item - you have to check if it's unique first</summary>
        public override void Rename(string newName)
        {
            Action.actionName = newName;
            displayName = newName;
        }

        public override void Delete(InputGroup group) =>
            group.actions.Remove(Action);

        public override bool CompareContent<t>(t item) =>
            item is InputAction a && Action == a;
    }

    public class InputMapContentAxisTreeItem : InputMapContentEditableItemBase
    {
        public InputAxis Axis { get; }

        public InputMapContentAxisTreeItem(InputAxis axis, int id)
        {
            Debug.Assert(axis != null, "Axis cannot be null");
            Axis = axis;
            displayName = Axis.axisName;
            this.id = id;
            depth = -1;
        }

        public override void Rename(string newName)
        {
            Axis.axisName = newName;
            displayName = newName;
        }

        public override void Delete(InputGroup group) =>
            group.axes.Remove(Axis);

        public override bool CompareContent<t>(t item) =>
            item is InputAxis a && Axis == a;
    }

    //This is just for recognition
    public abstract class InputMapContentHeaderItemBase : InputMapContentItemBase
    {
        public InputMapContentHeaderItemBase() : base() { }
        public InputMapContentHeaderItemBase(int id) : base(id) { }
        public InputMapContentHeaderItemBase(int id, int depth) : base(id, depth) { }
        public InputMapContentHeaderItemBase(int id, int depth, string displayName) : base(id, depth, displayName) { }
        public InputMapContentHeaderItemBase(int id, int depth, string displayName, Color barColor) : base(id, depth, displayName, barColor) { }
    }

    public class InputMapTreeActionHeaderItem : InputMapContentHeaderItemBase
    {
        public InputMapTreeActionHeaderItem() : base() { }
        public InputMapTreeActionHeaderItem(int id) : base(id) { }
        public InputMapTreeActionHeaderItem(int id, int depth) : base(id, depth) { }
        public InputMapTreeActionHeaderItem(int id, int depth, string displayName) : base(id, depth, displayName) { }
        public InputMapTreeActionHeaderItem(int id, int depth, string displayName, Color barColor) : base(id, depth, displayName, barColor) { }
    }

    public class InputMapTreeAxisHeaderItem : InputMapContentHeaderItemBase
    {
        public InputMapTreeAxisHeaderItem() : base() { }
        public InputMapTreeAxisHeaderItem(int id) : base(id) { }
        public InputMapTreeAxisHeaderItem(int id, int depth) : base(id, depth) { }
        public InputMapTreeAxisHeaderItem(int id, int depth, string displayName) : base(id, depth, displayName) { }
        public InputMapTreeAxisHeaderItem(int id, int depth, string displayName, Color barColor) : base(id, depth, displayName, barColor) { }
    }
}