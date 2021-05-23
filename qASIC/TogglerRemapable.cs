using qASIC.InputManagement;

namespace qASIC
{
	public class TogglerRemapable : Toggler
	{
        public string KeyName;

        private void Update()
        {
            if (InputManager.GetInputDown(KeyName))
                Toggle();
        }
    }
}