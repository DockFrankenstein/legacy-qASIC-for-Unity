using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    [System.Serializable] public class AssignEvent : UnityEngine.Events.UnityEvent<InputManagerKeys> { }

    public class InputAssign : MonoBehaviour
    {
        public string KeyName;
        public InputManagerKeys Keys;

        public AssignEvent OnAssign;

        public void Initialize(InputManagerKeys keys, string keyName)
        {
            KeyName = keyName;
            Keys = keys;
        }

        public void Assign(KeyCode key)
        {
            if (Keys != null) Keys = InputManager.ChangeInput(Keys, KeyName, key);
            else { InputManager.ChangeInput(KeyName, key); }
            if (Keys != null) OnAssign.Invoke(Keys);
        }
    }
}