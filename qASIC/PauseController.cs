using qASIC.Toggling;
using UnityEngine;

namespace qASIC
{
	public class PauseController : MonoBehaviour
	{
        public bool PauseTime;
        public bool LockCursor;
        public bool PauseAudio;

        private void Awake()
        {
            Toggler toggler = GetComponent<Toggler>();
            if (toggler == null) return;
            toggler.OnChangeState.AddListener(OnChangeState);
        }

        private void OnChangeState(bool state)
        {
            if (PauseTime) Time.timeScale = state ? 0f : 1f;
            if (LockCursor) Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;

            if (PauseAudio)
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