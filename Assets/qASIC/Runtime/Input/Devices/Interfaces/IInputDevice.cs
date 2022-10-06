using System;

namespace qASIC.Input.Devices
{
    public interface IInputDevice
    {
        string DeviceName { get; }
        Type KeyType { get; }

        float GetInputValue(string button);
        InputEventType GetInputEvent(string keyPath);
        string GetAnyKeyDown();

        void Initialize();
        void Update();
    }
}