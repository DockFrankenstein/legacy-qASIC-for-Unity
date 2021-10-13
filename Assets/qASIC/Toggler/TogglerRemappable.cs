using qASIC.InputManagement;

namespace qASIC.Toggling
{
	public class TogglerRemappable : Toggler
	{
#if ENABLE_INPUT_SYSTEM
        [Message("qASIC Input System only works with the Legacy Input System! Please, change the toggler or switch back to the old solution.", InspectorMessageIconType.error)]
#endif
        public string keyName;

#if !ENABLE_INPUT_SYSTEM
        private void Update()
        {
            if (InputManager.GetInputDown(keyName))
                KeyToggle();
        }
#endif
    }
}