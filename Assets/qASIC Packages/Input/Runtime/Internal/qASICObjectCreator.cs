using UnityEditor;
using UnityEngine;

namespace qASIC.Input.Internal
{
    public static partial class qASICObjectCreator
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/qASIC/Input assign", false, 2)]
        static void CreateInputAssign()
        {
            GameObject obj = new GameObject("Input Assign", typeof(Input.SetGlobalInputKeys));
            qASIC.Internal.qASICObjectCreator.FinishObject(obj);
        }
#endif
    }
}