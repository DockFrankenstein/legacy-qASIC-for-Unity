#if UNITY_EDITOR
using UnityEditor;
using qASIC.InputManagement.Internal.ReferenceExplorers;

namespace qASIC.InputManagement.Internal
{
    [CustomPropertyDrawer(typeof(InputActionReference))]
    public class InputActionReferenceDrawer : InputReferenceDrawerBase
    {
        protected override string ItemPropertyName => "actionName";
        protected override string ItemLabelName => "Action";

        public override void OnChangePressed(SerializedProperty property)
        {
            InputActionReferenceExplorerWindow.OpenProperty(property);
        }
    }
}
#endif