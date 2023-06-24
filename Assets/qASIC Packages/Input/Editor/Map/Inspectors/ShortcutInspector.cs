using System;
using System.Linq;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class ShortcutInspector : InputMapItemInspector
    {
        public override Type ItemType => typeof(Shortcut);

        Shortcut _shortcut;
        KeysDrawer _keyDrawer;

        public override void Initialize(OnInitializeContext context)
        {
            _shortcut = context.item as Shortcut;

            _keyDrawer = new KeysDrawer();
            _keyDrawer.Initialize(_shortcut.keys);
            _keyDrawer.OnDirty += () =>
            {
                SetMapDirty();
                window.Repaint();
            };
        }

        protected override void OnGUI(OnGUIContext context)
        {
            _keyDrawer.PathsHaveErrors = _shortcut.HasUnassignedPaths().Count() != 0;
            _keyDrawer.OnGUI();
        }

        protected override void HandleDeletion(OnGUIContext context)
        {
            map.groups
                .Where(x => x.items.Contains(context.item))
                .First()
                .RemoveItem(context.item as InputMapItem);
        }
    }
}
