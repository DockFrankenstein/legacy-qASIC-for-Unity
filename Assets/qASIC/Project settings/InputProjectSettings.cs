using qASIC.InputManagement.Map;
using qASIC.FileManagement;

namespace qASIC.ProjectSettings
{
    //[CreateAssetMenu(fileName = "NewInputProjectSettings", menuName = "qASIC/Project Setting Files/Input")]
    public class InputProjectSettings : ProjectSettingsBase
    {
        public static InputProjectSettings _instance;
        public static InputProjectSettings Instance => CheckInstance("Input", _instance);

        public InputMap map;
        public SerializationType serializationType = SerializationType.playerPrefs;
        public AdvancedGenericFilePath filePath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "input.txt", "input-editor.txt");

        public bool startArgsDisableLoad = true;
        public bool startArgsDisableSave = true;
    }
}