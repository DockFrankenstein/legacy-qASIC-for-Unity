using UnityEngine.Events;
using UnityEngine;
using System;

namespace qASIC
{
    [Serializable] public class UnityEventInt : UnityEvent<int> { }
    [Serializable] public class UnityEventString : UnityEvent<string> { }
    [Serializable] public class UnityEventFloat : UnityEvent<float> { }
    [Serializable] public class UnityEventDouble : UnityEvent<double> { }
    [Serializable] public class UnityEventBool : UnityEvent<bool> { }
    [Serializable] public class UnityEventKeyCode : UnityEvent<KeyCode> { }
}