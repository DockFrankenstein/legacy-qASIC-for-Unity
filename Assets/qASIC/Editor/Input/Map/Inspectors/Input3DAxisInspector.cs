using System;
using System.Linq;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class Input3DAxisInspector : InputMapItemInspector
    {
        public override Type ItemType => typeof(Input3DAxis);

        Input3DAxis _axis;

        public override void Initialize(OnInitializeContext context)
        {
            _axis = context.item as Input3DAxis;
        }

        protected override void OnGUI(OnGUIContext context)
        {
            InputGUIUtility.DrawAxis("X Axis", window, _axis.XAxis);
            InputGUIUtility.DrawAxis("Y Axis", window, _axis.YAxis);
            InputGUIUtility.DrawAxis("Z Axis", window, _axis.ZAxis);
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