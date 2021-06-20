using System.Collections;
using UnityEngine;

namespace qASIC.AudioManagment
{
    [System.Serializable]
    public class AudioChannel
    {
        public bool paused;
        public bool useGlobalControlls;

        public AudioSource source { get; set; }
        public IEnumerator destroyEnum { get; set; }

        public IEnumerator DestroyOnPlaybackEnd()
        {
            if (source == null || source.loop) yield break;
            float delay = 0.1f;
            yield return new WaitForSecondsRealtime(source.clip.length - source.time + delay);
            Object.Destroy(source);
            source = null;
            destroyEnum = null;
        }

        public AudioChannel()
        {
            source = null;
            destroyEnum = null;
        }
    }
}