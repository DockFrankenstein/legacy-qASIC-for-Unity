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

        public AudioSource target { get; private set; }

        private void Awake()
        {
            target = GetComponent<AudioSource>();
            OnStopAll += () => { target.Stop(); };
            OnPauseAll += () => { target.Pause(); };
            OnUnPauseAll += () => { target.UnPause(); };
        }
    }
}