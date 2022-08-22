using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qASIC.InputManagement
{
    public static class InputUpdateManager
    {
        static internal event Action OnUpdate;

        public static void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}
