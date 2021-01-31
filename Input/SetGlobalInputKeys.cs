using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    public class SetGlobalInputKeys : MonoBehaviour
    {
        public InputPreset preset;

        private void Awake() => InputManager.globalKeys = preset.preset;
    }
}