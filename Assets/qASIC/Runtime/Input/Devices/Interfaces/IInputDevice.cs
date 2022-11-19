﻿using System;
using System.Collections.Generic;

namespace qASIC.Input.Devices
{
    public interface IInputDevice
    {
        string DeviceName { get; set; }
        Type KeyType { get; }
        bool RuntimeOnly { get; }

        float GetInputValue(string button);
        InputEventType GetInputEvent(string keyPath);
        string GetAnyKeyDown();
        Dictionary<string, float> Values { get; }

        void Initialize();
        void Update();
    }
}