using UnityEngine;

namespace qASIC.InputManagement
{
    public class SetGlobalInputKeys : MonoBehaviour
    {
        public InputPreset Preset;
        private static bool init = false;

        private void Awake()
        {
            if (init) return;
            InputManager.GlobalKeys = Preset?.Preset;
            init = true;
        }
    }
}