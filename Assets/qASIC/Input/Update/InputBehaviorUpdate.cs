using UnityEngine;

namespace qASIC.InputManagement.Update
{
    internal class InputBehaviorUpdate : MonoBehaviour
    {
        private void Update()
        {
            InputUpdateManager.Update();
        }
    }
}