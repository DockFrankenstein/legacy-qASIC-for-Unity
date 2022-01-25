using qASIC.InputManagement;

namespace qASIC.Toggling
{
    public class StaticTogglerRemappable : StaticToggler
    {
#if !ENABLE_LEGACY_INPUT_MANAGER
        [Message("qASIC Input System only works with the Legacy Input System! Please, change the toggler or switch back to the old solution.", InspectorMessageIconType.error)]
#endif
        public string groupName;
        public string actionName;

#if ENABLE_LEGACY_INPUT_MANAGER
        private void Update()
        {
            if (InputManager.GetInputDown(groupName, actionName))
                KeyToggle();
        }
#endif
    }
}