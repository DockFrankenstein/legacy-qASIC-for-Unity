using System;

namespace qASIC.InputManagement.Devices
{
    public interface IInputDevice
    {
        string DeviceName { get; }
        Type KeyType { get; }

        float GetInputValue(string button);

        bool GetInputEvent(KeyEventType type, string keyPath);

        void Initialize();
        void Update();
    }

    public enum KeyEventType { down, up }
}