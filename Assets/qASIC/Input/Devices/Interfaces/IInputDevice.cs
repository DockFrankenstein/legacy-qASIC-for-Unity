using System;

namespace qASIC.InputManagement.Devices
{
    public interface IInputDevice
    {
        string DeviceName { get; }
        Type KeyType { get; }

        float GetInputValue(int button);

        bool GetInputEvent(KeyEventType type, int keyIndex);

        void Initialize();
        void Update();
    }

    public enum KeyEventType { down, up }
}