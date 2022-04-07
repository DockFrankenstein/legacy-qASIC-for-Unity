#if UNITY_EDITOR
using qASIC.Tools;
using System.Collections.Generic;
using UnityEditor;

using Manager = qASIC.InputManagement.Internal.EditorInputManager;

namespace qASIC.InputManagement.Internal.ReferenceExplorers
{
    public class InputActionReferenceExplorerWindow : InputReferenceExplorerBase
    {
        public override string ContentPropertyName => "actionName";

        public override List<INonRepeatable> GetContentList(int groupIndex) =>
            NonRepeatableChecker.GenerateNonRepeatableList(Manager.Map.Groups[groupIndex].actions);

        public static void OpenProperty(SerializedProperty property)
        {
            InputActionReferenceExplorerWindow window = CreateInstance(typeof(InputActionReferenceExplorerWindow)) as InputActionReferenceExplorerWindow;
            OpenWindow(window);
            window.Property = property;
            window.ResetEditor();
        }
    }
}
#endif