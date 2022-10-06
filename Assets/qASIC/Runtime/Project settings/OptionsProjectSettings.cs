using UnityEngine;
using qASIC.FileManagement;

namespace qASIC.ProjectSettings
{
    [System.Serializable]
    [ExcludeFromPreset]
    public class OptionsProjectSettings : ProjectSettingsBase
    {
        private static OptionsProjectSettings _instance;
        public static OptionsProjectSettings Instance => CheckInstance("Options", _instance);

        public bool enableOptionsSystem = true;
        public bool autoInitialize = true;

        public SerializationType serializationType = SerializationType.playerPrefs;
        public AdvancedGenericFilePath savePath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "settings.txt", "settings-editor.txt");

        [InspectorLabel("Disable Loading")] public bool startArgsDisableLoad = true;
        [InspectorLabel("Disable Saving")] public bool startArgsDisableSave = true;
        [InspectorLabel("Disable Settings")] public bool startArgsDisableInit = true;
    }
}