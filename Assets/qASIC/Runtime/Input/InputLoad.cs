using UnityEngine;
using qASIC.Input.Map;

namespace qASIC.Input
{
    [AddComponentMenu("qASIC/Input/Input Load")]
    public class InputLoad : MonoBehaviour
    {
        [Message("This feature has not been reimplemented yet :/", InspectorMessageIconType.error)]
        [SerializeField] InputMap map;

        private static bool init = false;

        private void Start()
        {
            if (init) return;

            if (!map)
            {
                qDebug.LogError("Cannot load Input Map: Input Map has not been assigned!");
                return;
            }

            init = true;
        }
    }
}