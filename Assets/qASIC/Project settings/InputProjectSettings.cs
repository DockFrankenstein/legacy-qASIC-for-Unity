using qASIC.InputManagement.Map;
using qASIC.FileManagement;
using qASIC.Options;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace qASIC.ProjectSettings
{
    [System.Serializable]
    [ExcludeFromPreset]
    //[CreateAssetMenu(fileName = "NewInputProjectSettings", menuName = "qASIC/Project Setting Files/Input")]
    public class InputProjectSettings : ScriptableObject
    {
        const string instanceLocation = "qASIC/Project Settings/Input";

        private static InputProjectSettings _instance;
        public static InputProjectSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    InputProjectSettings settings = Resources.Load<InputProjectSettings>(instanceLocation);

                    if (settings == null)
                        settings = CreateNewInstance();

                    _instance = settings;
                }

                return _instance;
            }
        }

        private static InputProjectSettings CreateNewInstance()
        {
#if UNITY_EDITOR
            InputProjectSettings asset = CreateInstance<InputProjectSettings>();
            AssetDatabase.CreateAsset(asset, $"{instanceLocation}.asset");
            AssetDatabase.SaveAssets();
            return asset;
#else
            throw new System.Exception("Cannot load qASIC project settings. Package has been modified or corrupted. Please reinstall or update!");
#endif
        }

        public InputMap map;
        public SerializationType serializationType = SerializationType.playerPrefs;
        public GenericFilePath filePath = new GenericFilePath(GenericFolder.PersistentDataPath, "input.txt");

        public bool startArgsDisableLoad = true;
        public bool startArgsDisableSave = true;
    }
}