using qASIC.InputManagement.Map;
using qASIC.FileManagement;
using qASIC.Options;

namespace qASIC.ProjectSettings
{
    //[CreateAssetMenu(fileName = "NewInputProjectSettings", menuName = "qASIC/Project Setting Files/Input")]
    public class InputProjectSettings : ProjectSettingsBase
    {
        public static InputProjectSettings _instance;
        public static InputProjectSettings Instance { get => CheckInstance("Input", _instance); }

        public InputMap map;
        public SerializationType serializationType = SerializationType.playerPrefs;
        public GenericFilePath filePath = new GenericFilePath(GenericFolder.PersistentDataPath, "input.txt");

        public bool startArgsDisableLoad = true;
        public bool startArgsDisableSave = true;
    }
}