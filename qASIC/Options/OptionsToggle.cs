using UnityEngine.UI;

namespace qASIC.Options.Menu
{
    public class OptionsToggle : MenuOption
    {
        private Toggle _toggle;

        public bool InvertEvent;
        public UnityEventBool OnValueChange = new UnityEventBool();

        private void Awake() => _toggle = GetComponent<Toggle>();

        public void SetValue(bool state) 
        { 
            SetValue(state, true); 
            OnValueChange.Invoke(state != InvertEvent); 
        }

        public override string GetLabel()
        {
            if (_toggle == null) return string.Empty;
            return $"{OptionLabelName}{_toggle.isOn}";
        }

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) ||
                !bool.TryParse(optionValue, out bool value) || _toggle == null) return;
            _toggle.SetIsOnWithoutNotify(value);
        }
    }
}