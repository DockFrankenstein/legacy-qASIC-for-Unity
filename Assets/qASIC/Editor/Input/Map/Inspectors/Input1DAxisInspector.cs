using System;
using System.Linq;
using UnityEditor;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class Input1DAxisInspector : InputMapItemInspector
    {
        public override Type ItemType => typeof(Input1DAxis);

        Input1DAxis _axis;

        public override void Initialize(OnInitializeContext context)
        {
            _axis = context.item as Input1DAxis;
        }

        protected override void OnGUI(OnGUIContext context)
        {
            _axis.positiveGuid = EditorGUILayout.DelayedTextField("Positive", _axis.positiveGuid);
            _axis.negativeGuid = EditorGUILayout.DelayedTextField("Negative", _axis.negativeGuid);
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
