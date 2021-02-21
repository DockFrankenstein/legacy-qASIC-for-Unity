using UnityEngine;
using UnityEngine.Audio;

namespace qASIC.AudioManagment
{
    [System.Serializable]
    public class AudioData
    {
        public AudioClip clip;
        public AudioMixerGroup group;
        public bool loop;
        public bool replace = true;
    }
}