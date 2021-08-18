using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagement
{
    [System.Serializable]
    public class InputManagerKeys
    {
        public string SavePath { get; set; }
        public Dictionary<string, KeyCode> Presets { get; set; }

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

        public InputManagerKeys(InputManagerKeys source)
        {
            SavePath = source.SavePath;
            Presets = new Dictionary<string, KeyCode>(source.Presets);
        }
    }
}