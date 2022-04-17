#if UNITY_EDITOR
//This script is only for the editor. We have to load audio manager parameters on start,
//because the audio mixer won't change parameters earlier in the editor
using UnityEngine;

namespace qASIC.AudioManagement.Internal
{
    [AddComponentMenu("")]
    public class AudioManagerEditorLoader : MonoBehaviour
    {
        private void Start()
        {
            AudioManager.LoadSettings();
            Destroy(gameObject);
        }
    }
}
#endif