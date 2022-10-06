using UnityEngine;

namespace qASIC.Input.Devices
{
    public interface IGamepadDevice : IInputDevice
    {
        void SetName(string name);
    }
}