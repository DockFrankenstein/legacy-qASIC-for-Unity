using System;
using UnityEngine;

namespace qASIC.Input.Devices
{
    [Serializable]
    public abstract class DeviceProvider
    {
        public string name;
        [HideInInspector] public RuntimePlatformFlags platforms = RuntimePlatformFlags.Everything;

        public string Name { get => name; set => name = value; }

        public virtual string DefaultItemName => "New Device Provider";
        public virtual RuntimePlatformFlags SupportedPlatforms => RuntimePlatformFlags.Everything;

        public virtual void Initialize()
        {

        }

        public virtual void Cleanup()
        {

        }

        public virtual void Update()
        {

        }
    }
}