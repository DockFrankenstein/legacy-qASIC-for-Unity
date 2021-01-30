using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    [CreateAssetMenu(fileName = "NewInputPreset", menuName = "Input Preset")]
    public class InputPreset : ScriptableObject
    {
        public InputManagerKeys preset = new InputManagerKeys();
    }
}