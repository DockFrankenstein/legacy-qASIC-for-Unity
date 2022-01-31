using UnityEngine;
using UnityEngine.Audio;
using qASIC.FileManagement;

namespace qASIC.ProjectSettings
{
    [System.Serializable]
    [ExcludeFromPreset]
    //[CreateAssetMenu(fileName = "NewOptionsProjectSettings", menuName = "qASIC/Project Setting Files/Options")]
    public class OptionsProjectSettings : ProjectSettingsBase
    {
        private static OptionsProjectSettings _instance;
        public static OptionsProjectSettings Instance => CheckInstance("Options", _instance);

        public SerializationType serializationType = SerializationType.playerPrefs;
        public AdvancedGenericFilePath savePath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "settings.txt", "settings-editor.txt");
    }
}