using qASIC.Input.Map;
using qASIC.Input.Devices;
using qASIC.Files.Serialization;
using qASIC.Files;

namespace qASIC.ProjectSettings
{
    public class InputProjectSettings : ProjectSettingsBase
    {
        public static InputProjectSettings _instance;
        public static InputProjectSettings Instance => CheckInstance("Input", _instance);

        public InputMap map;
        public DeviceStructure deviceStructure;

        public ObjectSerializer serializer = new ObjectSerializer(new JSONSerializationProvider());

        [InspectorLabel("Disable Loading")] public bool startArgsDisableLoad = true;
        [InspectorLabel("Disable Saving")] public bool startArgsDisableSave = true;

        public override void OnImport()
        {
#if UNITY_EDITOR
            var map = CreateInstance<InputMap>();
            UnityEditor.AssetDatabase.CreateAsset(map, "Assets/Cablebox Input Map.asset");
            this.map = map;

            var deviceStructure = CreateInstance<DeviceStructure>();
            UnityEditor.AssetDatabase.CreateAsset(deviceStructure, "Assets/Cablebox Device Structure.asset");

            deviceStructure.AddHandler(typeof(UIMKeyboardProvider));
            deviceStructure.AddHandler(typeof(XInputGamepadProvider));
            this.deviceStructure = deviceStructure;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}