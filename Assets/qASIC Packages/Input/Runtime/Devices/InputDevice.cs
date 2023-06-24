using qASIC.Internal;
using System;
using System.Collections.Generic;

namespace qASIC.Input.Devices
{
    public abstract class InputDevice : IInputDevice
    {
        public virtual string DeviceName { get; set; }

        public abstract Type KeyType { get; }

        public virtual bool RuntimeOnly => false;

        public abstract Dictionary<string, float> Values { get; }

        public abstract string GetAnyKeyDown();

        public abstract InputEventType GetInputEvent(string keyPath);

        public abstract float GetInputValue(string button);

        public virtual void Initialize()
        {

        }

        public virtual void Update()
        {

        }

        public virtual PropertiesList GetProperties()
        {
            return new PropertiesList();
        }
    }

    public abstract class InputDevice<T> : InputDevice
    {
        public override Type KeyType => typeof(T);
    }
}