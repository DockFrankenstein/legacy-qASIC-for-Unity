using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagement
{
    [System.Serializable]
    public class InputManagerKeys
    {
        public string SavePath;
        public Dictionary<string, KeyCode> Presets;

        public InputManagerKeys()
        {
            SavePath = "qASIC/Input.txt";
            Presets = new Dictionary<string, KeyCode>();
        }

        public InputManagerKeys(string savePath, Dictionary<string, KeyCode> presets)
        {
            SavePath = savePath;
            Presets = presets;
        }

        public InputManagerKeys(InputManagerKeys original)
        {
            SavePath = original.SavePath;
            Presets = new Dictionary<string, KeyCode>(original.Presets);
        }
    }
}