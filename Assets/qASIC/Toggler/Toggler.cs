using UnityEngine;

namespace qASIC.Toggling
{
    public class Toggler : MonoBehaviour
    {
        public bool State { get; private set; }
        public GameObject toggleObject;
        public KeyToggleMode keyMode;
        public UnityEventBool OnChangeState;

        public enum KeyToggleMode { Both, On, Off }

        private void Reset()
        {
            if (transform.childCount != 1) return;
            toggleObject = transform.GetChild(0).gameObject;
        }

        public virtual void Awake()
        {
            if (toggleObject == null) return;
            Toggle(toggleObject.activeSelf);
        }

        public virtual void Toggle() => Toggle(!State);

        public virtual void KeyToggle()
        {
            if (!State && keyMode == KeyToggleMode.Off || State && keyMode == KeyToggleMode.On) return;
            Toggle(!State);
        }

        public virtual void Toggle(bool state)
        {
            if (toggleObject == null) return;
            State = state;
            toggleObject.SetActive(state);
            OnChangeState?.Invoke(state);
        }
    }
}