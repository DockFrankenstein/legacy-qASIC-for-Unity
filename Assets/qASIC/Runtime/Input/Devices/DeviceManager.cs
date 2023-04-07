using System;
using System.Collections.Generic;
using UnityEngine;
using qASIC.ProjectSettings;
using qASIC.Input.Update;

namespace qASIC.Input.Devices
{
    public static class DeviceManager
    {
        public static List<DeviceProvider> Providers { get; private set; } = new List<DeviceProvider>();
        public static List<IInputDevice> Devices { get; private set; } = new List<IInputDevice>();

        //Callbacks
        public static event Action<int, IInputDevice> OnDeviceConnected;
        public static event Action<int, IInputDevice> OnDeviceDisconnected;

        public static IInputDevice LastUsedDevice { get; private set; }

        static bool _initialized;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Initialize()
        {
            if (_initialized)
#if UNITY_EDITOR
                Shutdown();
#else
                return;
#endif

            _initialized = true;
            qDebug.LogInternal("[Device Manager] Initializing...");

            InputUpdateManager.OnUpdate += Update;

            Devices.Clear();
            Providers.Clear();

            var newProviders = InputProjectSettings.Instance?.deviceStructure?.GetActiveProviders();
            if (newProviders != null)
                Providers = new List<DeviceProvider>(newProviders);

            foreach (var item in Providers)
            {
                item.Initialize();
                InputUpdateManager.OnUpdate += item.Update;
            }

            qDebug.LogInternal("[Device Manager] Initialization complete");
        }

        public static void Shutdown()
        {
            if (!_initialized)
                return;

            _initialized = false;
            qDebug.LogInternal("[Device Manager] Shutdown initiated...");

            InputUpdateManager.OnUpdate -= Update;

            foreach (var item in Providers)
            {
                item.Cleanup();
                InputUpdateManager.OnUpdate -= item.Update;
            }

            var devices = new List<IInputDevice>(Devices);
            foreach (var item in devices)
            {
                DeregisterDevice(item);
            }

            qDebug.LogInternal("[Device Manager] Shutdown complete");
        }

        public static void Reload()
        {
#if !UNITY_EDITOR
            Shutdown();
#endif
            Initialize();
        }

        public static void RegisterDevice(IInputDevice device)
        {
            Devices.Add(device);
            device.Initialize();

            InputUpdateManager.OnUpdate += device.Update;

            OnDeviceConnected?.Invoke(Devices.Count - 1, device);
        }

        public static void DeregisterDevice(IInputDevice device)
        {
            if (device == null)
                return;

            InputUpdateManager.OnUpdate -= device.Update;

            int deviceIndex = Devices.IndexOf(device);
            if (deviceIndex != -1)
                Devices.RemoveAt(deviceIndex);

            OnDeviceDisconnected?.Invoke(deviceIndex, device);
        }

        static void Update()
        {
            foreach (var device in Devices)
            {
                if (string.IsNullOrEmpty(device.GetAnyKeyDown()))
                    continue;

                LastUsedDevice = device;
                break;
            }
        }
    }
}