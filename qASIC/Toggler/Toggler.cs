using UnityEngine;

namespace qASIC.Toggling
{
    public abstract class Toggler : MonoBehaviour
    {
        public bool state { get; private set; }
        public GameObject ToggleObject;
        public KeyToggleMode KeyMode;
        public UnityEventBool OnChangeState;

        public enum KeyToggleMode { both, on, off }

        private void Reset()
        {
            if (transform.childCount != 1) return;
            ToggleObject = transform.GetChild(0).gameObject;
        }

        public virtual void Awake() => Toggle(ToggleObject.activeSelf);

        public virtual void Toggle() => Toggle(!state);

        public virtual void KeyToggle()
        {
            if (!state && KeyMode == KeyToggleMode.off || state && KeyMode == KeyToggleMode.on) return;
            Toggle(!state);
        }

        public virtual void Toggle(bool state)
        {
            this.state = state;
            ToggleObject?.SetActive(state);
            OnChangeState.Invoke(state);
        }
    }
}