using qASIC.EditorTools.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

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
            _axis.XAxis = InputMapWindowUtility.DrawAxis("X Axis", _axis.XAxis, map);
            _axis.YAxis = InputMapWindowUtility.DrawAxis("Y Axis", _axis.YAxis, map);
            _axis.ZAxis = InputMapWindowUtility.DrawAxis("Z Axis", _axis.ZAxis, map);
        }

        protected override void HandleDeletion(OnGUIContext context)
        {
            var group = map.groups
                .Where(x => x.items.Contains(context.item))
                .First()
                .items
                .Remove(context.item as InputMapItem);
        }
    }
}
