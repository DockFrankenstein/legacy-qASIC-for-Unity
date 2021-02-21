using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    [CreateAssetMenu(fileName = "NewInputPreset", menuName = "qASIC/Input/Input Preset")]
    public class InputPreset : ScriptableObject, ISerializationCallbackReceiver
    {
        public InputManagerKeys Preset = new InputManagerKeys();

        private List<string> _keys = new List<string>();
        private List<KeyCode> _values = new List<KeyCode>();

        public void OnAfterDeserialize()
        {
            Preset.Presets = new Dictionary<string, KeyCode>();
            for (int i = 0; i < Mathf.Min(_keys.Count, _values.Count); i++)
                if(!Preset.Presets.ContainsKey(_keys[i]))
                    Preset.Presets.Add(_keys[i], _values[i]);
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (KeyValuePair<string, KeyCode> entry in Preset.Presets)
            {
                _keys.Add(entry.Key);
                _values.Add(entry.Value);
            }
        }
    }
}