using System;
using UnityEditor;
using UnityEngine;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class InputGroupInspector : InputMapItemInspector
    {
        InputGroup _group;

        public override Type ItemType => typeof(InputGroup);

        public override void Initialize(OnInitializeContext context)
        {
            _group = context.item as InputGroup;
        }

        protected override void OnGUI(OnGUIContext context)
        {
            bool isDefault = map.groups.IndexOf(_group) == map.defaultGroup;
            using (new EditorGUI.DisabledScope(isDefault))
            {
                if (GUILayout.Button("Set as default"))
                {
                    int index = map.groups.IndexOf(_group);

                    if (index != -1)
                        map.defaultGroup = index;
                }
            }
        }

        protected override void OnDebugGUI(OnGUIContext context)
        {
            base.OnDebugGUI(context);
            bool isDefault = map.groups.IndexOf(_group) == map.defaultGroup;
            GUILayout.Label($"Item Count: {_group.items.Count}");
            GUILayout.Label($"Is Default: {isDefault}");
        }

        protected override bool CanDelete(OnGUIContext context) =>
            map.groups.Count > 1 &&
            map.groups.IndexOf(context.item as InputGroup) != map.defaultGroup;

        protected override void HandleDeletion(OnGUIContext context)
        {
            window.DeleteGroup(_group);
        }
    }
}
