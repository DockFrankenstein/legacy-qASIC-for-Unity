using qASIC.Toggling;
using qASIC.Toggling.Controllers;
using UnityEngine;

namespace qASIC
{
    [AddComponentMenu("qASIC/Menu/Pause Controller")]
    public class PauseController : MonoBehaviour
    {
        public bool pauseTime = true;
        public bool lockCursor = true;
        public bool pauseAudio = true;

        [SerializeField] [InspectorLabel("Toggler")] MonoBehaviour togglerObject;

        IToggable _toggler;

        bool _isQuitting;

        private void Reset()
        {
            togglerObject = GetComponent<IToggable>() as MonoBehaviour;
        }

        private void OnValidate()
        {
            if (togglerObject != null && !(togglerObject is IToggable))
            {
                Debug.LogError("Toggler must implement IToggable!");
                togglerObject = null;
            }
        }

        private void Awake()
        {
            if (!(togglerObject is IToggable toggler)) return;
            _toggler = toggler;

            _toggler.OnToggle += OnChangeState;
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

            if (_toggler == null) return;
            if (!_toggler.State) return;

            //Reset pause controller
            OnChangeState(false);
        }

        public void Toggle(bool state) =>
            _toggler?.Toggle(state);

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