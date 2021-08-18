using System.Collections;
using UnityEngine;

namespace qASIC.AudioManagment
{
    [System.Serializable]
    public class AudioChannel
    {
        public bool paused;
        public bool useGlobalControlls;

        public AudioSource Source { get; set; }
        public IEnumerator DestroyEnum { get; set; }

        public IEnumerator DestroyOnPlaybackEnd()
        {
            if (Source == null || Source.loop) yield break;
            yield return new WaitForSecondsRealtime(Source.clip.length - Source.time + 0.1f);
            Object.Destroy(Source);
            Source = null;
            DestroyEnum = null;
        }

        public AudioChannel()
        {
            Source = null;
            DestroyEnum = null;
        }
    }
}