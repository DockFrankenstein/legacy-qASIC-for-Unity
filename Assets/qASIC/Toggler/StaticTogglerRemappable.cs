using qASIC.InputManagement;
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
            Debug.Log(InputManager.GetInputValue<float>(item.GetGroupName(), item.GetItemName()));
            if (InputManager.GetInputDown(item.GetGroupName(), item.GetItemName()))
                KeyToggle();
        }
    }
}