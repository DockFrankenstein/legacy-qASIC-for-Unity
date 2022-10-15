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
            _axis.XAxis = DrawAxis("X Axis", _axis.XAxis);
            _axis.YAxis = DrawAxis("Y Axis", _axis.YAxis);
        }

        Axis DrawAxis(string label, Axis axis)
        {
            EditorGUILayout.Space();
            using (new qGUIInternalUtility.GroupScope(label))
            {
                axis.axisGuid = EditorGUILayout.DelayedTextField("Axis", axis.axisGuid);
                EditorGUILayout.Space();

                bool isUsingAxis = axis.IsUsingAxis();
                Input1DAxis targetAxis = axis.IsUsingAxis() ? map.GetItem<Input1DAxis>(axis.axisGuid) : null;

                using (new EditorGUI.DisabledGroupScope(isUsingAxis))
                {
                    string newPositive = EditorGUILayout.DelayedTextField("Positive", targetAxis?.positiveAction ?? axis.positiveGuid);
                    string newNegative = EditorGUILayout.DelayedTextField("Negative", targetAxis?.negativeAction ?? axis.negativeGuid);

                    if (targetAxis == null)
                    {
                        axis.positiveGuid = newPositive;
                        axis.negativeGuid = newNegative;
                    }
                }
            }

            return axis;
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