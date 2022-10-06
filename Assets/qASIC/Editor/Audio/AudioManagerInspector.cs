#if UNITY_EDITOR
using UnityEditor;

namespace qASIC.Audio.Internal
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerInspector : Editor
    {
        public override void OnInspectorGUI() { }
    }
}
#endif