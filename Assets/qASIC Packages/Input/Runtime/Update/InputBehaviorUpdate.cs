using UnityEngine;

namespace qASIC.Input.Update
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