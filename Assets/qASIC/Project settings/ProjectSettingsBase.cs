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

        private static t CreateNewInstance<t>(string assetName) where t : ProjectSettingsBase
        {
#if UNITY_EDITOR
            t asset = CreateInstance<t>();
            AssetDatabase.CreateAsset(asset, $"{instanceLocation}/{assetName}.asset");
            AssetDatabase.SaveAssets();
            return asset;
#else
            throw new System.Exception("Cannot load qASIC project settings. Package has been modified or corrupted. Please reinstall or update!");
#endif
        }
    }
}