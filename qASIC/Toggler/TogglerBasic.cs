using UnityEngine;

namespace qASIC.Toggling
{
    public class TogglerBasic : Toggler
    {
        public KeyCode Key = KeyCode.F2;

        private void Update()
        {
            if (Input.GetKeyDown(Key))
                KeyToggle();
        }
    }
}