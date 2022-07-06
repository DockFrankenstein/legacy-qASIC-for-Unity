using UnityEngine;

namespace qASIC.Toggling
{
    [AddComponentMenu("qASIC/Togglers/Toggler Basic")]
    public class TogglerBasic : Toggler
    {
#if ENABLE_INPUT_SYSTEM
        public UnityEngine.InputSystem.Key key = UnityEngine.InputSystem.Key.F2;
#else
        [KeyCodeListener]
        public KeyCode key = KeyCode.F2;
#endif

        protected override void HandleInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (key != UnityEngine.InputSystem.Key.None && 
                UnityEngine.InputSystem.Keyboard.current[key].wasPressedThisFrame)
#else
            if (Input.GetKeyDown(key))
#endif
                KeyToggle();
        }
    }
}