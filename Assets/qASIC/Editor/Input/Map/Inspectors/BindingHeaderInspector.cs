using UnityEngine;
using UnityEditor;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class BindingHeaderInspector : InputMapItemInspector
    {
        protected override void OnGUI(OnGUIContext context)
        {
            GUILayout.Label("Create", EditorStyles.whiteLargeLabel);
            if (GUILayout.Button("Create New Binding"))
                window.AddItem(new InputBinding());
        }
    }
}