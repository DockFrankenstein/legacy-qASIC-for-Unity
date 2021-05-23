using UnityEngine;

namespace qASIC
{
    public class TogglerBasic : Toggler
    {
        public KeyCode Key = KeyCode.F2;

        private void Update()
        {
            if (Input.GetKeyDown(Key))
                Toggle();
        }
    }
}