using System;
using System.Collections.Generic;

namespace qASIC.InputManagement.Devices
{
    public static class DeviceManager
    {
        public static List<IInputDevice> Devices { get; private set; } = new List<IInputDevice>();

        //Callbacks
        public static event Action<int, IInputDevice> OnDeviceConnected;
        public static event Action<int, IInputDevice> OnDeviceDisconnected;

        public static void RegisterDevice(IInputDevice device)
        {
            Devices.Add(device);
            device.Initialize();
            OnDeviceConnected?.Invoke(Devices.Count - 1, device);
        }

        public static void DeregisterDevice(IInputDevice device)
        {
            int deviceIndex = Devices.IndexOf(device);
            Devices.RemoveAt(deviceIndex);
            OnDeviceDisconnected?.Invoke(deviceIndex, device);
        }
    }
}