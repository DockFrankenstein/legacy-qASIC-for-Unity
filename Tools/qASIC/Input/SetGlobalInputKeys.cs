using UnityEngine;

namespace qASIC.InputManagement
{
    public class SetGlobalInputKeys : MonoBehaviour
    {
        public InputPreset preset;
        private static bool init = false;

        private void Awake()
        {
            if (init || preset == null) return;
            InputManager.GlobalKeys = preset?.preset;
            InputManager.LoadUserKeys();
            init = true;
        }
    }
}