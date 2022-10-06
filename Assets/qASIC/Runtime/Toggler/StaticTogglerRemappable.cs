using qASIC.Input;
using UnityEngine;

namespace qASIC.Toggling
{
    [AddComponentMenu("qASIC/Togglers/Static Toggler Remappable")]
    public class StaticTogglerRemappable : StaticToggler
    {
        [Space]
        public InputMapItemReference item;

        protected override void HandleInput()
        {
            if (InputManager.GetInputDown(item.GetGroupName(), item.GetItemName()))
                KeyToggle();
        }
    }
}