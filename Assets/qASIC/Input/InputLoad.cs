using UnityEngine;
using qASIC.FileManagement;
using qASIC.InputManagement.Map;

namespace qASIC.InputManagement
{
    [AddComponentMenu("qASIC/Input/Input Load")]
    public class InputLoad : MonoBehaviour
    {
        [SerializeField] InputMap map;

        [SerializeField] SerializationType serializationType = SerializationType.playerPrefs;
        [SerializeField] AdvancedGenericFilePath filePath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "input.txt", "input-editor.txt");

        private static bool init = false;

        private void Start()
        {
            if (init) return;

            if (!map)
            {
                qDebug.LogError("Cannot load Input Map: Input Map has not been assigned!");
                return;
            }

            InputManager.LoadMap(map);
            
            switch (serializationType)
            {
                case SerializationType.playerPrefs:
                    InputManager.LoadUserKeysPrefs();
                    break;
                case SerializationType.config:
                    InputManager.LoadUserKeysConfig(filePath.GetFullPath());
                    break;
                case SerializationType.none:
                    break;
                default:
                    qDebug.LogError($"Serialization type '{serializationType}' is not supported by the input system!");
                    break;
            }

            init = true;
        }
    }
}