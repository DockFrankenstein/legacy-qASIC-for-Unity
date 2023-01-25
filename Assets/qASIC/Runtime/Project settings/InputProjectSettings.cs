using qASIC.Input.Map;
using qASIC.FileManagement;
using qASIC.Input.UIM;
using qASIC.Input.Devices;

namespace qASIC.ProjectSettings
{
    public class InputProjectSettings : ProjectSettingsBase
    {
        public static InputProjectSettings _instance;
        public static InputProjectSettings Instance => CheckInstance("Input", _instance);

        public InputMap map;
        public DeviceStructure deviceStructure;

        [UnityEngine.SerializeReference] public Serialization.SerializationProvider serializationProvider;

        public SerializationType serializationType = SerializationType.playerPrefs;
        public AdvancedGenericFilePath filePath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "input.txt", "input-editor.txt");

        [InspectorLabel("Disable Loading")] public bool startArgsDisableLoad = true;
        [InspectorLabel("Disable Saving")] public bool startArgsDisableSave = true;
    }
}