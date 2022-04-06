using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace qASIC.ProjectSettings
{
    [System.Serializable]
    [ExcludeFromPreset]
    public abstract class ProjectSettingsBase : ScriptableObject
    {
        const string instanceLocation = "qASIC/Project Settings";
        const string defaultInstanceLocation = "qASIC/Project Settings/Default";

        protected static t CheckInstance<t>(string assetName, t instance) where t : ProjectSettingsBase
        {
            if (instance == null)
            {
                t settings = Resources.Load<t>($"{instanceLocation}/{assetName}");

                if (settings == null)
                    settings = CreateNewInstance<t>(assetName);

                instance = settings;
            }

            return instance;
        }

        private static t GetDefaultInstance<t>(string assetName) where t : ProjectSettingsBase
        {
            t asset = Resources.Load<t>($"{defaultInstanceLocation}/{assetName}");

            if (asset == null)
            {
#if qASIC_DEV && UNITY_EDITOR
                asset = CreateNewDefaultInstance<t>(assetName);
#else
                throw new System.Exception("[qASIC] Cannot load qASIC project settings. Package has been modified or corrupted. Please reinstall or update!");        
#endif
            }

            return asset;
        }

        private static t CreateNewInstance<t>(string assetName) where t : ProjectSettingsBase
        {
#if UNITY_EDITOR && !qASIC_DEV
            t asset = CreateInstance<t>();
            AssetDatabase.CreateAsset(asset, $"Assets/qASIC/Resources/{instanceLocation}/{assetName}.asset");
            AssetDatabase.SaveAssets();
            return asset;
#else
            return GetDefaultInstance<t>(assetName);
#endif
        }

#if UNITY_EDITOR
        private static t CreateNewDefaultInstance<t>(string assetName) where t : ProjectSettingsBase
        {
            t asset = CreateInstance<t>();
            AssetDatabase.CreateAsset(asset, $"Assets/qASIC/Resources/{defaultInstanceLocation}/{assetName}.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
#endif
    }
}