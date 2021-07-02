using UnityEngine;

namespace qASIC.Toggling
{
    public class StaticTogglerBasic : StaticToggler
    {
        public KeyCode Key = KeyCode.F2;

        private void Update()
        {
            if (Input.GetKeyDown(Key))
                KeyToggle();
        }
    }
}