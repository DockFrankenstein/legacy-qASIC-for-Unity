using qASIC.InputManagement;

namespace qASIC.Toggling
{
    public class StaticTogglerRemappable : StaticToggler
    {
        public string keyName;

        private void Update()
        {
            if (InputManager.GetInputDown(keyName))
                KeyToggle();
        }
    }
}