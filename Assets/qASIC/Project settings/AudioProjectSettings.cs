using UnityEngine;
using UnityEngine.Audio;
using qASIC.FileManagement;

namespace qASIC.ProjectSettings
{
    [System.Serializable]
    [ExcludeFromPreset]
    public class AudioProjectSettings : ProjectSettingsBase
    {
        private static AudioProjectSettings _instance;
        public static AudioProjectSettings Instance => CheckInstance("Audio", _instance);

        public bool enableAudioManager = false;
        public string managerName = "Audio Manager";
        public AudioMixer mixer;

        [Space]
        public bool createOnStart = true;
        public bool createOnUse = true;

        //Logging creation
        [InspectorLabel("Log")] public bool logCreation = true;
        [InspectorLabel("Message")] [TextArea(3, 5)] public string creationLogMessage = "Audio Manager successfully created!";
        [InspectorLabel("Message Color")] public string creationLogColor = "audio";
        [Space]
        [InspectorLabel("Log Error")] public bool logCreationError = true;
        [InspectorLabel("Error message")] [TextArea(3, 5)] public string creationErrorMessage = "Couldn't create Audio Manager: ";
        [InspectorLabel("Error color")] public string creationErrorColor = "error";

        //Saving
        public SerializationType serializationType = SerializationType.config;
        public AdvancedGenericFilePath savePath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "audio.txt", "audio-editor.txt");

        //Other
        [Tooltip("Determines if parameters should have rounded values")] public bool roundValues = true;
    }
}