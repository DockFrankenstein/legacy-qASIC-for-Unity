using UnityEngine;
using System;

namespace qASIC.AudioManagment
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceController : MonoBehaviour
    {
        public static Action OnStopAll = new Action(() => { });
        public static Action OnPauseAll = new Action(() => { });
        public static Action OnUnPauseAll = new Action(() => { });

        public AudioSource Target { get; private set; }

        private void Awake()
        {
            Target = GetComponent<AudioSource>();
            if (Target == null) return;
            OnStopAll += () => { Target.Stop(); };
            OnPauseAll += () => { Target.Pause(); };
            OnUnPauseAll += () => { Target.UnPause(); };
        }
    }
}