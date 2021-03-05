using UnityEngine.UI;

namespace qASIC.Options.Menu
{
    public class OptionsSlider : MenuOption
    {
        private Slider _slider;

        private void Awake() => _slider = GetComponent<Slider>();
        public void PreviewValue(float value) => SetValueSlider(value, false);
        public void SetValue(float value) => SetValueSlider(value, true);

        public void SetValue()
        {
            if(_slider != null) SetValue(_slider.value);
        }

        private void SetValueSlider(float value, bool log)
        {
            if (_slider.wholeNumbers && int.TryParse(value.ToString(), out int intResult))
            {
                SetValue(intResult, log);
                return;
            }
            SetValue(value, log);
        }

        private void Update()
        {
            if (NameText != null && _slider != null) NameText.text = GetPropertyName(_slider.value);
        }

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) || 
                !float.TryParse(optionValue, out float value) || _slider == null) return;
            _slider.value = value;
            isActive = true;
        }
    }
}