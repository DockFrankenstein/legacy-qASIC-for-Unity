using qASIC.InputManagement;

namespace qASIC.Toggling
{
	public class TogglerRemappable : Toggler
	{
        public string KeyName;

        private void Update()
        {
            if (InputManager.GetInputDown(KeyName))
                KeyToggle();
        }
    }
}