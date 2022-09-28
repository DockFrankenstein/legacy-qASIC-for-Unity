using UnityEngine;

namespace qASIC.InputManagement.Devices
{
    public interface IGamepadDevice : IInputDevice
    {
        void SetName(string name);
    }
}