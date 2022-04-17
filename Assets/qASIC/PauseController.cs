using qASIC.Toggling;
using UnityEngine;

namespace qASIC
{
    public class PauseController : MonoBehaviour
    {
        public bool pauseTime = true;
        public bool lockCursor = true;
        public bool pauseAudio = true;

        Toggler toggler;

        bool _isQuitting;

        private void Awake()
        {
            toggler = GetComponent<Toggler>();
            toggler?.OnChangeState.AddListener(OnChangeState);
        }

        private void OnDisable()
        {
            ResetPause();
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        void ResetPause()
        {
            if (_isQuitting) return;

            if (toggler == null) return;
            if (!toggler.State) return;

            //Reset pause controller
            OnChangeState(false);
        }

        public void Toggle(bool state) =>
            toggler?.Toggle(state);

        private void OnChangeState(bool state)
        {
            if (pauseTime)
                Time.timeScale = state ? 0f : 1f;

            if (lockCursor)
            {
                Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = state;
            }

            if (pauseAudio)
            {
                switch (state)
                {
                    case true:
                        AudioManagement.AudioManager.PauseAll();
                        break;
                    default:
                        AudioManagement.AudioManager.UnPauseAll();
                        break;
                }
            }
        }
    }
}