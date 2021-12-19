using UnityEngine;
using System;
using System.Collections.Generic;

namespace qASIC.InputManagement
{
    [Serializable]
    public class InputAction : INonRepeatable
    {
        public string actionName;

        public List<KeyCode> keys = new List<KeyCode>();

        public string ItemName { get => actionName; }

        public InputAction() { }
        
        public InputAction(string name)
        {
            actionName = name;
        }

        public bool GetInput() =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKey(key); }));

        public bool GetInputDown() =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }));

        public bool GetInputUp() =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKey(key); }));

        public bool HandleInput(Func<KeyCode, bool> statement)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (statement.Invoke(keys[i]))
                    return true;
            }

            return false;
        }

        public KeyCode GetKey(int index)
        {
            TryGetKey(index, out KeyCode key, true);
            return key;
        }

        public bool TryGetKey(int index, out KeyCode key, bool logError = false)
        {
            if (index >= 0 && keys.Count < index)
            {
                key = keys[index];
                return true;
            }

            key = KeyCode.None;
            if (logError)
                qDebug.LogError($"Action does not contain key <b>{index}</b>");

            return false;
        }
    }
}