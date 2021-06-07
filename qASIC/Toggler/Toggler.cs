using UnityEngine;

namespace qASIC.Toggling
{
    public abstract class Toggler : MonoBehaviour
    {
        public bool state { get; private set; }
        public GameObject ToggleObject;
        public UnityEventBool OnChangeState;

        private void Reset()
        {
            if (transform.childCount != 1) return;
            ToggleObject = transform.GetChild(0).gameObject;
        }

        public virtual void Awake() => Toggle(ToggleObject.activeSelf);

        public virtual void Toggle() => Toggle(!state);

        public virtual void Toggle(bool state)
        {
            this.state = state;
            ToggleObject?.SetActive(state);
            OnChangeState.Invoke(state);
        }
    }
}