using System;

namespace qASIC.Toggling
{
    public interface IToggable
    {
        Action<bool> OnToggle { get; set; }
        bool State { get; }

        void Toggle();
        void Toggle(bool state);
    }
}