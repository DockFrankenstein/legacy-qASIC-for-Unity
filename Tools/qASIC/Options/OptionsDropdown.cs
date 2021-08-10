using TMPro;

namespace qASIC.Options.Menu
{
    public class OptionsDropdown : MenuOption
    {
        private TMP_Dropdown dropdown;

        private void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            if (dropdown != null) dropdown.onValueChanged.AddListener((int index) => SetValue(index, true));
        }

        public override string GetLabel()
        {
            if (dropdown == null || dropdown.value >= dropdown.options.Count) return string.Empty;
            return $"{nameText}{dropdown.options[dropdown.value].text}";
        }

        public override void LoadOption()
        {
            if (dropdown == null) return;
            if (!OptionsController.TryGetUserSetting(optionName, out string optionValue) ||
                !int.TryParse(optionValue, out int result)) return;

            if (result >= dropdown.options.Count) return;
            dropdown.SetValueWithoutNotify(result);
        }
    }
}
