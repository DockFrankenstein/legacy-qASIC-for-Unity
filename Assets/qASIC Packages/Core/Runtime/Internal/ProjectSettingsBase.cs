using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace qASIC.Internal
{
    [System.Serializable]
    [ExcludeFromPreset]
    public abstract class ProjectSettingsBase : ScriptableObject
    {
        const string instanceLocation = "qASIC/Project Settings";

        protected static t CheckInstance<t>(string assetName, t instance) where t : ProjectSettingsBase
        {
            if (instance == null)
            {
                var settings = Resources.Load<t>($"{instanceLocation}/{assetName}");

#if UNITY_EDITOR
                if (settings == null)
                    settings = CreateNewInstance<t>(assetName);
#endif

                instance = settings;
            }

            return instance;
        }

        private static t CreateNewInstance<t>(string assetName) where t : ProjectSettingsBase
        {
#if UNITY_EDITOR
            var path = $"{Application.dataPath}/qASIC/Resources/{instanceLocation}";
            var directoryExists = Directory.Exists(path);

            if (!directoryExists)
                Directory.CreateDirectory(path);

            var asset = CreateInstance<t>();
            AssetDatabase.CreateAsset(asset, $"Assets/qASIC/Resources/{instanceLocation}/{assetName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
#endif
        }
    }
}