using UnityEngine;
using System;

namespace qASIC.Toggling
{
    [AddComponentMenu("qASIC/Togglers/Toggler")]
    public class Toggler : MonoBehaviour, IToggable
    {
        private bool _state;
        public bool State => _state;

        public GameObject toggleObject;
        public KeyToggleMode keyMode;
        public UnityEventBool OnChangeState;

        public TogglerMode mode;

        public Action<bool> OnToggle { get; set; }

        public enum KeyToggleMode { Both, On, Off }
        public enum TogglerMode { Controler, Module }

        private void Reset()
        {
            if (transform.childCount != 1) return;
            toggleObject = transform.GetChild(0).gameObject;
        }

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Update()
        {
            if (mode == TogglerMode.Controler)
                HandleInput();
        }

        protected virtual void HandleInput() { }

        public virtual void Initialize()
        {
            if (toggleObject == null) return;
            Toggle(toggleObject.activeSelf);
        }

        public void ForceInputHandle()
        {
            if (mode == TogglerMode.Module)
                HandleInput();
        }

        public virtual void Toggle() => Toggle(!_state);

        public virtual void KeyToggle()
        {
            if (!_state && keyMode == KeyToggleMode.Off || _state && keyMode == KeyToggleMode.On) return;
            Toggle(!_state);
        }

        public virtual void Toggle(bool state)
        {
            if (toggleObject == null) return;
            _state = state;
            toggleObject.SetActive(state);
            OnToggle?.Invoke(state);
            OnChangeState?.Invoke(state);
        }
    }
}