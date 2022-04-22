using qASIC.InputManagement;
using UnityEngine;

namespace qASIC.Toggling
{
    [AddComponentMenu("qASIC/Togglers/Toggler Remappable")]
    public class TogglerRemappable : Toggler
	{
#if !ENABLE_LEGACY_INPUT_MANAGER
        [Message("qASIC Input System only works with the Legacy Input System! Please, change the toggler or switch back to the old solution.", InspectorMessageIconType.error)]
#endif
        public InputActionReference action;

#if ENABLE_LEGACY_INPUT_MANAGER
        protected override void HandleInput()
        {
            if (InputManager.GetInputDown(action))
                KeyToggle();
        }
#endif
    }
}