using UnityEngine;

namespace qASIC.Toggling
{
    public abstract class Toggler : MonoBehaviour
    {
        public bool state { get; private set; }
        public GameObject ToggleObject;
        public UnityEventBool OnChangeState;

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