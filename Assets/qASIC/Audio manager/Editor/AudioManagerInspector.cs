#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace qASIC.AudioManagement.Internal
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerInspector : Editor
    {
        public override void OnInspectorGUI() { }
    }
}
#endif