using UnityEngine;

namespace qASIC
{
    public abstract class Toggler : MonoBehaviour
    {
        public bool state { get; private set; }
        public GameObject ToggleObject;
        public UnityEventBool OnChangeState;

        private void Awake() => Toggle(ToggleObject.activeSelf);

        public void Toggle() => Toggle(!state);

        public void Toggle(bool state)
        {
            this.state = state;
            ToggleObject?.SetActive(state);
            OnChangeState.Invoke(state);
        }
    }
}