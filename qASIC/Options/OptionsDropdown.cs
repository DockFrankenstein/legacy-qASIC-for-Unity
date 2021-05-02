using TMPro;

namespace qASIC.Options.Menu
{
    public class OptionsDropdown : MenuOption
    {
        private TMP_Dropdown _dropdown;

        public override string GetLabel()
        {
            if (_dropdown == null || _dropdown.value >= _dropdown.options.Count) return string.Empty;
            return $"{NameText}{_dropdown.options[_dropdown.value].text}";
        }

        public override void LoadOption()
        {
            if (_dropdown == null) return;
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) ||
                !int.TryParse(optionValue, out int result)) return;

            if (result >= _dropdown.options.Count) return;
            _dropdown.SetValueWithoutNotify(result);
        }
    }
}
