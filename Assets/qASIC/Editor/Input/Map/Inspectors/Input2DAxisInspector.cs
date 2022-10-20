using System;
using System.Collections.Generic;
using System.Linq;
using qASIC.EditorTools.Internal;
using UnityEditor;

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
            _axis.XAxis = InputMapWindowUtility.DrawAxis("X Axis", _axis.XAxis, map);
            _axis.YAxis = InputMapWindowUtility.DrawAxis("Y Axis", _axis.YAxis, map);
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