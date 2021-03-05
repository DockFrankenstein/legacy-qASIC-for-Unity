using UnityEngine.UI;

namespace qASIC.Options.Menu
{
    public class OptionsToggle : MenuOption
    {
        private Toggle _toggle;

        public bool invertEvent;
        public UnityEventBool OnValueChange = new UnityEventBool();

        private void Awake() => _toggle = GetComponent<Toggle>();

        public void SetValue(bool state) 
        { 
            SetValue(state, true); 
            OnValueChange.Invoke(state != invertEvent); 
        }

        private void Update()
        {
            if (NameText != null && _toggle != null) NameText.text = GetPropertyName(_toggle.isOn);
        }

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) ||
                !bool.TryParse(optionValue, out bool value) || _toggle == null)
            {
                isActive = true;
                return;
            }
            _toggle.isOn = value;
            isActive = true;
        }
    }
}