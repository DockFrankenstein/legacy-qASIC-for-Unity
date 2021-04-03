using UnityEngine;

namespace qASIC.InputManagement
{
    public class SetGlobalInputKeys : MonoBehaviour
    {
        public InputPreset Preset;

        private void Awake() => InputManager.GlobalKeys = Preset?.Preset;
    }
}