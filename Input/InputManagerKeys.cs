using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    public class InputManagerKeys
    {
        public string savePath;
        public Dictionary<string, KeyCode> presets;

        public InputManagerKeys()
        {
            savePath = "qASIC/Input.txt";
            presets = new Dictionary<string, KeyCode>();
        }

        public InputManagerKeys(string _savePath, Dictionary<string, KeyCode>  _presets)
        {
            savePath = _savePath;
            presets = _presets;
        }
    }
}