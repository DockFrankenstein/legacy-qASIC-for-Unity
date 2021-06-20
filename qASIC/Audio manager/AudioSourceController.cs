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
            OnStopAll += OnStop;
            OnPauseAll += OnPause;
            OnUnPauseAll += OnUnPause;
            if (AudioManager.Paused) Target.Pause();
        }

        void OnStop() => Target?.Stop();
        void OnPause() => Target?.Pause();
        void OnUnPause() => Target?.UnPause();

        private void OnDestroy()
        {
            OnStopAll -= OnStop;
            OnPauseAll -= OnPause;
            OnUnPauseAll -= OnUnPause;
        }
    }
}