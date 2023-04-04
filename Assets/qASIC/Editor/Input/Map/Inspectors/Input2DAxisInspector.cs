using System;
using System.Linq;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class Input2DAxisInspector : InputMapItemInspector
    {
        public override Type ItemType => typeof(Input2DAxis);

        Input2DAxis _axis;

        public override void Initialize(OnInitializeContext context)
        {
            _axis = context.item as Input2DAxis;
        }

        protected override void OnGUI(OnGUIContext context)
        {
            InputGUIUtility.DrawAxis("X Axis", window, _axis.XAxis);
            InputGUIUtility.DrawAxis("Y Axis", window, _axis.YAxis);
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