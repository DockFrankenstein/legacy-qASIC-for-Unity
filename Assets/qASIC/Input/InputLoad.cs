using UnityEngine;
using qASIC.FileManagement;
using qASIC.Options;

namespace qASIC.InputManagement
{
    public class InputLoad : MonoBehaviour
    {
        [SerializeField] InputMap map;

        [SerializeField] SerializationType serializationType = SerializationType.playerPrefs;
        [SerializeField] GenericFilePath filePath = new GenericFilePath(GenericFolder.PersistentDataPath, "input.txt");

        bool init = false;

        private void Awake()
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
                default:
                    qDebug.LogError($"Serialization type '{serializationType}' is not supported by the input system!");
                    break;
            }

            init = true;
        }
    }
}