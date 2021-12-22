using UnityEngine;

namespace qASIC.Toggling
{
    public class TogglerBasic : Toggler
    {
#if ENABLE_INPUT_SYSTEM
        public UnityEngine.InputSystem.Key key = UnityEngine.InputSystem.Key.F2;
#else
        [KeyCodeListener]
        public KeyCode key = KeyCode.F2;
#endif

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current[key].wasPressedThisFrame)
#else
            if (Input.GetKeyDown(key))
#endif
                KeyToggle();
        }
    }
}