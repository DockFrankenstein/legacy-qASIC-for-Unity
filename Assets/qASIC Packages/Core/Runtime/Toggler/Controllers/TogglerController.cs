using UnityEngine;
using System;

namespace qASIC.Toggling.Controllers
{
    public abstract class TogglerController : MonoBehaviour, IToggable
    {
        public Toggler CurrentToggler { get; private set; }
        public bool State => CurrentToggler?.State == true;

        protected virtual void HandleInput() { }

        public Action<bool> OnToggle
        {
            get => CurrentToggler?.OnToggle;
            set
            {
                if (CurrentToggler == null) return;
                CurrentToggler.OnToggle = value;
            }
        }

        protected void ChangeToggler(Toggler toggler)
        {
            ChangeTogglerSilent(toggler);
            CurrentToggler?.Initialize();
        }

        protected void ChangeTogglerSilent(Toggler toggler)
        {
            if (CurrentToggler != null)
                toggler?.Toggle(CurrentToggler.State);

            CurrentToggler = toggler;
        }

        void HandleToggle(bool state) =>
            OnToggle?.Invoke(state);

        public void Toggle() =>
            CurrentToggler?.Toggle();

        public void Toggle(bool state) =>
            CurrentToggler?.Toggle(state);

        private void Update()
        {
            HandleInput();
        }
    }
}