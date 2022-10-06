using System;
using UnityEngine;

namespace qASIC.Input.Update
{
    public static class InputUpdateManager
    {
        public static event Action OnUpdate;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            new GameObject("[qASIC] Input Update", typeof(InputBehaviorUpdate), typeof(AddToDontDestroy));
        }

        public static void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}
