#if UNITY_EDITOR
using qASIC.Tools;
using System.Collections.Generic;
using UnityEditor;

using Manager = qASIC.InputManagement.Internal.EditorInputManager;

namespace qASIC.InputManagement.Internal.ReferenceExplorers
{
    public class InputAxisReferenceExplorerWindow : InputReferenceExplorerBase
    {
        public override string ContentPropertyName => "axisName";

        public override List<INonRepeatable> GetContentList(int groupIndex) =>
            NonRepeatableChecker.GenerateNonRepeatableList(Manager.Map.Groups[groupIndex].axes);

        public static void OpenProperty(SerializedProperty property)
        {
            InputAxisReferenceExplorerWindow window = CreateInstance(typeof(InputAxisReferenceExplorerWindow)) as InputAxisReferenceExplorerWindow;
            OpenWindow(window);
            window.Property = property;
            window.ResetEditor();
        }
    }
}

#endif