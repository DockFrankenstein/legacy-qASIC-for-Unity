using TMPro;
using UnityEngine;

namespace qASIC.Options.Menu
{
    [AddComponentMenu("qASIC/Options/Dropdown")]
    public class OptionsDropdown : MenuOption
    {
        public TMP_Dropdown dropdown;

        private void Reset()
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }

        protected virtual void Awake()
        {
            if (dropdown == null)
                dropdown = GetComponent<TMP_Dropdown>();

            if (dropdown != null)
                dropdown.onValueChanged.AddListener((int index) => SetValue(index, true));
        }

        public override string GetLabel()
        {
            if (dropdown == null || dropdown.value >= dropdown.options.Count) return string.Empty;
            return $"{optionLabelName}{dropdown.options[dropdown.value].text}";
        }

        public override void LoadOption()
        {
            if (dropdown == null) return;
            if (!OptionsController.TryGetOptionValue(optionName, out string optionValue) ||
                !int.TryParse(optionValue, out int result)) return;

            if (result >= dropdown.options.Count) return;
            dropdown.SetValueWithoutNotify(result);
        }
    }
}