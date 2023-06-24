#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using qASIC.EditorTools;
using qASIC.Input.Map.Internal.Inspectors;
using UnityEditor.PackageManager.UI;

namespace qASIC.Input.Map.Internal
{
    public class InputMapContentItemBase : TreeViewItem
    {
        public InputMapContentItemBase() : base() { }
        public InputMapContentItemBase(int id) : base(id) { }
        public InputMapContentItemBase(int id, int depth) : base(id, depth) { }


        public virtual Texture GetIcon(InputGroup group) => null;
        public virtual string GetTooltip(InputGroup group) => string.Empty;
        public virtual Color ItemColor { get; set; } = Color.clear;
        public virtual bool CanDrag { get => false; }

        public virtual void SelectInInspector(InputMapWindow window)
        {
            window.SelectInInspector(null);
        }

        public virtual void CreateGenericMenu(GenericMenu menu, InputMapWindowContentTree contentTree) { }
    }

    public class InputMapContentBindingHeader : InputMapContentItemBase
    {
        public InputMapContentBindingHeader() : base() { }
        public InputMapContentBindingHeader(int id) : base(id) { }
        public InputMapContentBindingHeader(int id, int depth) : base(id, depth) { }

        public override void SelectInInspector(InputMapWindow window)
        {
            window.SetInspector(new BindingHeaderInspector());
        }
    }

    public class InputMapContentOtherHeader : InputMapContentItemBase
    {
        public InputMapContentOtherHeader() : base() { }
        public InputMapContentOtherHeader(int id) : base(id) { }
        public InputMapContentOtherHeader(int id, int depth) : base(id, depth) { }

        public override void SelectInInspector(InputMapWindow window)
        {
            window.SetInspector(new OtherHeaderInspector());
        }
    }

    public class InputMapContentMapItem : InputMapContentItemBase
    {
        public InputMapContentMapItem() : base()
        {
            Guid = System.Guid.NewGuid().ToString();
            id = Guid.GetHashCode();
        }

        public InputMapContentMapItem(InputMapItem item) : base((item?.Guid ?? System.Guid.NewGuid().ToString()).GetHashCode())
        {
            Item = item;
            displayName = item?.ItemName ?? string.Empty;
        }

        public string Guid { get; set; }
        public InputMapItem Item { get; set; }
        public override bool CanDrag => true;
        public override Color ItemColor => Item?.ItemColor ?? Color.clear;

        public override void SelectInInspector(InputMapWindow window)
        {
            window.SelectInInspector(Item);
        }

        public override Texture GetIcon(InputGroup group) =>
            Item?.HasErrors() == true ? qGUIEditorUtility.ErrorIcon : null;

        public override void CreateGenericMenu(GenericMenu menu, InputMapWindowContentTree contentTree)
        {
            menu.AddSeparator("");
            foreach (var group in contentTree.window.Map.groups)
                menu.AddItem($"Move to group/{group.ItemName}", group.items.Contains(Item), () => 
                {
                    if (group.items.Contains(Item))
                        return;

                    contentTree.Group.RemoveItem(Item);
                    group.AddItem(Item);
                    contentTree.Reload();
                    contentTree.window.SetMapDirty();
                });

            menu.AddItem("Copy Guid", false, () => GUIUtility.systemCopyBuffer = Item?.Guid);
        }
    }
}
#endif