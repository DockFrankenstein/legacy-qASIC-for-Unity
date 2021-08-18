using qASIC.InputManagement;

namespace qASIC.Toggling
{
	public class TogglerRemappable : Toggler
	{
        public string keyName;

        private void Update()
        {
            if (InputManager.GetInputDown(keyName))
                KeyToggle();
        }
    }
}