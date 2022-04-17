using UnityEngine;
using qASIC.FileManagement;

namespace qASIC.Options
{
    [AddComponentMenu("qASIC/Options/Options Load")]
    public class OptionsLoad : MonoBehaviour
    {
        [SerializeField] SerializationType serializationType = SerializationType.playerPrefs;
        [SerializeField] AdvancedGenericFilePath filePath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "settings.txt", "settings-editor.txt");

        private static bool init = false;

        private void Start()
        {
            if (init) return;

            switch (serializationType)
            {
                case SerializationType.playerPrefs:
                    OptionsController.OverrideEnabled(true);
                    OptionsController.Initialize(false);
                    OptionsController.LoadPrefs();
                    break;
                case SerializationType.config:
                    OptionsController.OverrideEnabled(true);
                    OptionsController.Initialize(false);
                    OptionsController.LoadConfig(filePath.ToString());
                    break;
                case SerializationType.none:
                    break;
                default:
                    qDebug.LogError($"Serialization type '{serializationType}' is not supported by the options system!");
                    break;
            }

            init = true;
        }
    }
}