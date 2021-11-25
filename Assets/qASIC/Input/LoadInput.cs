using UnityEngine;

namespace qASIC.InputManagement
{
    public class LoadInput : MonoBehaviour
    {
        public InputMap map;

        bool init = false;

        private void Awake()
        {
            if (init) return;
            init = true;
            InputManager.LoadMap(map);
        }
    }
}