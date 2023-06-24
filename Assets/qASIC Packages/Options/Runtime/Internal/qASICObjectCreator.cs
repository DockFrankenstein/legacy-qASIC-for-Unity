using UnityEditor;
using UnityEngine;

using static qASIC.Internal.qASICObjectCreator;

namespace qASIC.SettingsSystem.Internal
{
    public static partial class qASICObjectCreator
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/qASIC/Options load", false, 2)]
        static void CreateOptionsLoad()
        {
            GameObject obj = new GameObject("Options load", typeof(OptionsLoad));
            FinishObject(obj);
        }
#endif
    }
}