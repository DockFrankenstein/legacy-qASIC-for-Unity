using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Input.Devices
{
    [CreateAssetMenu(fileName = "NewDeviceStructure", menuName = "qASIC/Input/Device Structure")]
    public class DeviceStructure : ScriptableObject
    {
        [SerializeReference] public List<DeviceProvider> providers = new List<DeviceProvider>();
    }
}