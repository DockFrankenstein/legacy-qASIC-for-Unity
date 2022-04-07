#if UNITY_EDITOR
using UnityEditor;
using qASIC.InputManagement.Internal.ReferenceExplorers;

namespace qASIC.InputManagement.Internal
{
    [CustomPropertyDrawer(typeof(InputAxisReference))]
    public class InputAxisReferenceDrawer : InputReferenceDrawerBase
    {
        protected override string ItemPropertyName => "axisName";
        protected override string ItemLabelName => "Axis";

        public override void OnChangePressed(SerializedProperty property)
        {
            InputAxisReferenceExplorerWindow.OpenProperty(property);
        }
    }
}
#endif