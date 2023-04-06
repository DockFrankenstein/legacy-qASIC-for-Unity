using System;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Input.Devices
{
    [CreateAssetMenu(fileName = "NewDeviceStructure", menuName = "qASIC/Input/Device Structure")]
    public class DeviceStructure : ScriptableObject
    {
        [SerializeReference] public List<DeviceProvider> providers = new List<DeviceProvider>();

#if UNITY_EDITOR
        /// <summary>Editor only method for adding new handlers</summary>
        public void AddHandler(Type type)
        {
            var provider = (DeviceProvider)Activator.CreateInstance(type, new object[] { });
            provider.Name = provider.DefaultItemName;

            providers.Add(provider);

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}