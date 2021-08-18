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

        private void Awake()
        {
            toggler = GetComponent<Toggler>();
            toggler?.OnChangeState.AddListener(OnChangeState);
        }

        public void Toggle(bool state) => toggler?.Toggle(state);

        private void OnChangeState(bool state)
        {
            if (pauseTime) Time.timeScale = state ? 0f : 1f;
            if (lockCursor) Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;

            if (pauseAudio)
            {
                switch(state)
                {
                    case true:
                        AudioManagment.AudioManager.PauseAll();
                        break;
                    default:
                        AudioManagment.AudioManager.UnPauseAll();
                        break;
                }
            }
        }
    }
}