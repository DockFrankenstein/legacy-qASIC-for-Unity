using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    [System.Serializable] public class AssignEvent : UnityEngine.Events.UnityEvent<InputManagerKeys> { }

    public class InputAssign : MonoBehaviour
    {
        public string keyName;
        public InputManagerKeys keys;

        public AssignEvent OnAssign;

        public void Initialize(InputManagerKeys _keys, string _keyName)
        {
            keyName = _keyName;
            keys = _keys;
        }

        public void Assign(KeyCode key)
        {
            Debug.Log(keys == null);
            if (keys != null) keys = InputManager.ChangeInput(keys, keyName, key);
            else { InputManager.ChangeInput(keyName, key); }
            if (keys != null) OnAssign.Invoke(keys);
        }
    }
}