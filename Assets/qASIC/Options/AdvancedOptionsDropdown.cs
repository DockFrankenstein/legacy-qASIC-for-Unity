using System.Collections.Generic;
using TMPro;

namespace qASIC.Options.Menu
{
    public abstract class AdvancedOptionsDropdown : MenuOption
    {
        [UnityEngine.HideInInspector] public TMP_Dropdown dropdown;
        public List<object> properties = new List<object>();

        public override void Start()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            if (dropdown == null) return;
            dropdown.onValueChanged.AddListener((int index) => SetValue(properties[index]));

            AssignDropdownOptions();
            LoadOption();
        }

        public virtual void SetValue(object property) => SetValue(property, true);

        public virtual void AssignDropdownOptions()
        {
            List<TMP_Dropdown.OptionData> dropDownData = new List<TMP_Dropdown.OptionData>();
            dropdown.ClearOptions();
            for (int i = 0; i < properties.Count; i++)
                dropDownData.Add(new TMP_Dropdown.OptionData() { text = GetDropdownValueName(properties[i]) });
            dropdown.AddOptions(dropDownData);
        }

        public virtual string GetDropdownValueName(object property) => property.ToString();
        public override string GetLabel()
        {
            if (dropdown == null || dropdown.value >= dropdown.options.Count) return string.Empty;
            return $"{optionLabelName}{dropdown.options[dropdown.value].text}";
        }

        public override void LoadOption()
        {
            if (dropdown == null) return;
            if (!OptionsController.TryGetUserSetting(optionName, out string optionValue) ||
                !int.TryParse(optionValue, out int value)) return;
            dropdown.SetValueWithoutNotify(value);
        }
    }
}