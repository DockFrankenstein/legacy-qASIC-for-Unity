using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    [ExecuteInEditMode]
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewInputPreset", menuName = "Input Preset")]
    public class InputPreset : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public InputManagerKeys preset = new InputManagerKeys();

        public List<string> _keys;
        public List<KeyCode> _values;

        public void OnAfterDeserialize()
        {
            preset.presets = new Dictionary<string, KeyCode>();
            for (int i = 0; i < Mathf.Min(_keys.Count, _values.Count); i++)
                preset.presets.Add(_keys[i], _values[i]);
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (KeyValuePair<string, KeyCode> entry in preset.presets)
            {
                _keys.Add(entry.Key);
                _values.Add(entry.Value);
            }
        }
    }
}