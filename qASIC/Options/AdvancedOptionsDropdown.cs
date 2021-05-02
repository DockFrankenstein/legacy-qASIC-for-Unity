using System.Collections.Generic;
using TMPro;

namespace qASIC.Options.Menu
{
    public abstract class AdvancedOptionsDropdown : MenuOption
    {
        [UnityEngine.HideInInspector] public TMP_Dropdown _dropdown;
        public List<object> properties = new List<object>();

        public override void Start()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            if (_dropdown == null) return;

            Assign();
            AssignDropdownOptions();
            SetCurrentIndex();
            LoadOption();
        }

        public abstract void Assign();

        public void SetValue(int value) => SetValue(properties[value]);
        public virtual void SetValue(object property) => SetValue(property, true);

        public virtual void AssignDropdownOptions()
        {
            List<TMP_Dropdown.OptionData> dropDownData = new List<TMP_Dropdown.OptionData>();
            _dropdown.ClearOptions();
            for (int i = 0; i < properties.Count; i++)
                dropDownData.Add(new TMP_Dropdown.OptionData() { text = GetDropdownValueName(properties[i]) });
            _dropdown.AddOptions(dropDownData);
        }

        public abstract void SetCurrentIndex();
        public virtual string GetDropdownValueName(object propertie) => propertie.ToString();
        public override string GetLabel()
        {
            if (_dropdown == null || _dropdown.value >= _dropdown.options.Count) return string.Empty;
            return $"{OptionLabelName}{_dropdown.options[_dropdown.value].text}";
        }

        public override void LoadOption()
        {
            if (_dropdown == null) return;
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) ||
                !int.TryParse(optionValue, out int value)) return;
            _dropdown.SetValueWithoutNotify(value);
        }
    }
}