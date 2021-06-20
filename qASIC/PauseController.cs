using qASIC.Toggling;
using UnityEngine;

namespace qASIC
{
	public class PauseController : MonoBehaviour
	{
        public bool PauseTime;
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
            if (PauseAudio)
            {
                switch(state)
                {
                    case true:
                        AudioManagment.AudioManager.UnPauseAll();
                        break;
                    default:
                        AudioManagment.AudioManager.PauseAll();
                        break;
                }
            }
        }
    }
}