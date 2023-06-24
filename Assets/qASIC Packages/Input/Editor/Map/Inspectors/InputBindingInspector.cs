using System;
using System.Linq;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class InputBindingInspector : InputMapItemInspector
    {
        public override Type ItemType => typeof(InputBinding);

        InputBinding _binding;

        KeysDrawer _keyDrawer;

        public override void Initialize(OnInitializeContext context)
        {
            _binding = context.item as InputBinding;

            _keyDrawer = new KeysDrawer();
            _keyDrawer.Initialize(_binding.keys);
            _keyDrawer.OnDirty += () =>
            {
                SetMapDirty();
                window.Repaint();
            };
        }

        protected override void OnGUI(OnGUIContext context)
        {
            _keyDrawer.PathsHaveErrors = _binding.HasUnassignedPaths().Count() != 0;
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