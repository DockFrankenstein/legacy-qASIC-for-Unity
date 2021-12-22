using UnityEngine;
using qASIC.FileManagement;

namespace qASIC.InputManagement
{
    public class InputLoad : MonoBehaviour
    {
        public InputMap map;

        public GenericFilePath filePath;

        bool init = false;

        private void Awake()
        {
            if (init) return;

            if (!map)
            {
                qDebug.LogError("Cannot load Input Map: Input Map has not been assigned!");
                return;
            }

            init = true;
            InputManager.LoadMap(map);
        }
    }
}