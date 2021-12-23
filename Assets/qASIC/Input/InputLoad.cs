using UnityEngine;
using qASIC.FileManagement;
using qASIC.Options;

namespace qASIC.InputManagement
{
    public class InputLoad : MonoBehaviour
    {
        public InputMap map;

        public SerializationType serializationType = SerializationType.playerPrefs;
        public GenericFilePath filePath = new GenericFilePath("input.txt");

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
                    qDebug.LogError("Serialization type '{SaveType}' is not supported by the input system!");
                    break;
            }

            init = true;
        }
    }
}