using UnityEngine;

namespace qASIC.InputManagement.Update
{
    [AddComponentMenu("")]
    internal class InputBehaviorUpdate : MonoBehaviour
    {
        private void Update()
        {
            InputUpdateManager.Update();
        }
    }
}