using qASIC.Toggling;
using UnityEngine;

namespace qASIC
{
	public class PauseController : MonoBehaviour
	{
        public bool PauseTime = true;
        public bool LockCursor = true;
        public bool PauseAudio = true;

        private void Awake()
        {
            GetComponent<Toggler>()?.OnChangeState.AddListener(OnChangeState);
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