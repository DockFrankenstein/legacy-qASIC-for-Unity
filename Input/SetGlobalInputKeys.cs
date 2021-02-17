using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    public class SetGlobalInputKeys : MonoBehaviour
    {
        public InputPreset Preset;

        private void Awake() => InputManager.GlobalKeys = Preset?.Preset;
    }
}