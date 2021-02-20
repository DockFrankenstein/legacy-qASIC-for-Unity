using UnityEngine.UI;

namespace qASIC.Options.UI
{
    public class OptionsToggle : MenuOption
    {
        private Toggle _toggle;

        public bool invertEvent;
        public UnityEventBool OnValueChange = new UnityEventBool();

        private void Awake() => _toggle = GetComponent<Toggle>();
        public void SetValue(bool state) { SetValue(state, true); OnValueChange.Invoke(state != invertEvent); }

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) || 
                !bool.TryParse(optionValue, out bool value) || _toggle == null) return;
            _toggle.isOn = value;
        }
    }
}