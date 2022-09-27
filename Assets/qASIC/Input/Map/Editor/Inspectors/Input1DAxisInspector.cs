using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static qASIC.InputManagement.Map.InputBinding;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;
using UnityEngine;

namespace qASIC.InputManagement.Map.Internal.Inspectors
{
    public class Input1DAxisInspector : InputMapItemInspector
    {
        public override Type ItemType => typeof(Input1DAxis);

        Input1DAxis _axis;
        bool _delete;

        public override void Initialize(OnInitializeContext context)
        {
            _axis = context.item as Input1DAxis;
        }

        public override void OnGUI(OnGUIContext context)
        {
            _axis.itemName = EditorGUILayout.DelayedTextField("Name", _axis.itemName);

            _axis.positiveAction = EditorGUILayout.DelayedTextField("Positive", _axis.positiveAction);
            _axis.negativeAction = EditorGUILayout.DelayedTextField("Negative", _axis.negativeAction);

            _delete = DeleteButton(_delete, context);
        }

        protected override void HandleDeletion(OnGUIContext context)
        {
            var group = map.groups
                .Where(x => x.items.Contains(context.item))
                .First()
                .items
                .Remove(context.item);
        }
    }
}
