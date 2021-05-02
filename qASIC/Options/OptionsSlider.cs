using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

namespace qASIC.Options.Menu
{
    public class OptionsSlider : MenuOption, IPointerDownHandler, IPointerUpHandler
    {
        private Slider _slider;

        public bool Round;
        [Tooltip("Fe. Round value 0.1 will round the value to 1 decimal place")]
        public double RoundValue;

        private void Awake() => _slider = GetComponent<Slider>();

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_slider == null) return;
            SetValueSlider(_slider.value, false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_slider == null) return;
            SetValueSlider(_slider.value, true);
        }

        private void SetValueSlider(float value, bool log)
        {
            if (_slider.wholeNumbers && int.TryParse(value.ToString(), out int intResult))
            {
                SetValue(intResult, log);
                return;
            }
            SetValue(Round ? Mathf.Round(value / (float)RoundValue) * RoundValue : value, log);
        }

        public override string GetLabel()
        {
            if (_slider == null) return string.Empty;
            return $"{OptionLabelName}{(Round ? Mathf.Round(_slider.value / (float)RoundValue) * RoundValue : _slider.value)}";
        }

        public override void LoadOption()
        {
            if (_slider == null) return;
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) || 
                !float.TryParse(optionValue, out float value)) return;
            _slider.SetValueWithoutNotify(value);
        }
    }
}