using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagement
{
    [CreateAssetMenu(fileName = "NewInputPreset", menuName = "qASIC/Input/Input Preset")]
    public class InputPreset : ScriptableObject, ISerializationCallbackReceiver
    {
        public InputManagerKeys preset = new InputManagerKeys();

        [SerializeField] List<string> _keys = new List<string>();
        [SerializeField] List<KeyCode> _values = new List<KeyCode>();

        public void OnAfterDeserialize()
        {
            preset.Presets = new Dictionary<string, KeyCode>();
            for (int i = 0; i < Mathf.Min(_keys.Count, _values.Count); i++)
                if(!preset.Presets.ContainsKey(_keys[i]))
                    preset.Presets.Add(_keys[i], _values[i]);
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (KeyValuePair<string, KeyCode> entry in preset.Presets)
            {
                _keys.Add(entry.Key);
                _values.Add(entry.Value);
            }
        }
    }
}