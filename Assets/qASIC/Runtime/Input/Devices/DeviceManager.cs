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

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Initialize()
        {
#if UNITY_EDITOR
            Shutdown();
#endif

            qDebug.LogInternal("[Device Manager] Initializing...");
            Providers.Clear();
            var newProviders = InputProjectSettings.Instance?.deviceStructure?.providers;
            if (newProviders != null)
                Providers = new List<DeviceProvider>(newProviders);

            foreach (var item in Providers)
            {
                item.Initialize();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.update += item.Update;
#else
                InputUpdateManager.OnUpdate += item.Update;
#endif
            }

            foreach (var item in Devices)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.update += item.Update;
#else
                InputUpdateManager.OnUpdate += item.Update;
#endif
            }

            qDebug.LogInternal("[Device Manager] Initialization complete");
        }

        public static void Shutdown()
        {
            qDebug.LogInternal("[Device Manager] Shutdown initiated...");
            foreach (var item in Providers)
            {
                item.Cleanup();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.update -= item.Update;
#else
                InputUpdateManager.OnUpdate -= item.Update;
#endif
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

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += device.Update;
#else
            InputUpdateManager.OnUpdate += device.Update;
#endif

            OnDeviceConnected?.Invoke(Devices.Count - 1, device);
        }

        public static void DeregisterDevice(IInputDevice device)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= device.Update;
#else
            InputUpdateManager.OnUpdate -= device.Update;
#endif

            int deviceIndex = Devices.IndexOf(device);
            Devices.RemoveAt(deviceIndex);
            OnDeviceDisconnected?.Invoke(deviceIndex, device);
        }
    }
}