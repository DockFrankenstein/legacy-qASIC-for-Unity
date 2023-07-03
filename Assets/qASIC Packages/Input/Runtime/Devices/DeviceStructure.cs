using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace qASIC.Input.Devices
{
    [CreateAssetMenu(fileName = "NewDeviceStructure", menuName = "qASIC/Input/Device Structure")]
    public class DeviceStructure : ScriptableObject
    {
        public List<DeviceProvider> Providers
        {
            get => providers;
            set => providers = value;
        }

        [SerializeReference] List<DeviceProvider> providers = new List<DeviceProvider>();

        public List<DeviceProvider> GetActiveProviders() =>
            providers
                .Where(x => x != null)
                .Where(x => x.SupportedPlatforms.HasFlag(qApplication.Platform))
                .Where(x => x.platforms.HasFlag(qApplication.Platform))
                .ToList();

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