using UnityEngine;

namespace qASIC.Toggling
{
    public class StaticTogglerBasic : StaticToggler
    {
        public KeyCode key = KeyCode.F2;

        private void Update()
        {
            if (Input.GetKeyDown(key))
                KeyToggle();
        }
    }
}