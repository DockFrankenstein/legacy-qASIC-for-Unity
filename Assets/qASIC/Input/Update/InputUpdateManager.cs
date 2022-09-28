using System;

namespace qASIC.InputManagement
{
    public static class InputUpdateManager
    {
        public static event Action OnUpdate;

        public static void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}
