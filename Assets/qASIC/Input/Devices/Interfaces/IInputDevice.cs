using System;

namespace qASIC.InputManagement.Devices
{
    public interface IInputDevice
    {
        string DeviceName { get; }
        Type KeyType { get; }

        float GetInputValue(string button);

        InputEventType GetInputEvent(string keyPath);

        void Initialize();
        void Update();
    }
}