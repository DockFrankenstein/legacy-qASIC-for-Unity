using UnityEngine;
using UnityEditor;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class OtherHeaderInspector : InputMapItemInspector
    {
        InputMapWindowUtility.ItemType[] _creatableItems;

        public override void Initialize(OnInitializeContext context)
        {
            base.Initialize(context);
            _creatableItems = InputMapWindowUtility.GetOtherItemTypesWithNames();
        }

        protected override void OnGUI(OnGUIContext context)
        {
            GUILayout.Label("Create", EditorStyles.whiteLargeLabel);

            foreach (var item in _creatableItems)
            {
                string name = item.name;
                if (GUILayout.Button($"Create New {name}"))
                    window.AddItem(item.type);
            }
        }
    }
}
