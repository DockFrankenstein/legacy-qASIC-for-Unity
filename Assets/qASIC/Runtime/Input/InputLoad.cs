using UnityEngine;
using qASIC.FileManagement;
using qASIC.Input.Map;

namespace qASIC.Input
{
    [AddComponentMenu("qASIC/Input/Input Load")]
    public class InputLoad : MonoBehaviour
    {
        [SerializeField] InputMap map;

#pragma warning disable CS0414
        [SerializeField] SerializationType serializationType = SerializationType.playerPrefs;
#pragma warning restore
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
            InputManager.SavePath = filePath.GetFullPath();
            InputManager.LoadPreferences();
            
            //switch (serializationType)
            //{
            //    case SerializationType.playerPrefs:
            //        InputManager.LoadUserKeysPrefs();
            //        break;
            //    case SerializationType.config:
            //        InputManager.LoadUserKeysConfig(filePath.GetFullPath());
            //        break;
            //    case SerializationType.none:
            //        break;
            //    default:
            //        qDebug.LogError($"Serialization type '{serializationType}' is not supported by the input system!");
            //        break;
            //}

            init = true;
        }
    }
}