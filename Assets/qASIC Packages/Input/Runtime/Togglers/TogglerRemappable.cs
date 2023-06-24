using qASIC.Input;
using UnityEngine;

namespace qASIC.Toggling
{
    [AddComponentMenu("qASIC/Togglers/Toggler Remappable")]
    public class TogglerRemappable : Toggler
	{
        [Space]
        public InputMapItemReference item;

#if ENABLE_LEGACY_INPUT_MANAGER
        protected override void HandleInput()
        {
            if (InputManager.GetInputDown(item.GetGroupName(), item.GetItemName()))
                KeyToggle();
        }
#endif
    }
}