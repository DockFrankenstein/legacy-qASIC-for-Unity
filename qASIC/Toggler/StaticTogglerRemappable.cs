using qASIC.InputManagement;

namespace qASIC.Toggling
{
    public class StaticTogglerRemappable : StaticToggler
    {
        public string KeyName;

        private void Update()
        {
            if (InputManager.GetInputDown(KeyName))
                KeyToggle();
        }
    }
}