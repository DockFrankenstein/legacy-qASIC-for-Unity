using System.Collections.Generic;
using TMPro;

namespace qASIC.Options.Menu
{
    public abstract class OptionsDropdown : MenuOption
    {
        [UnityEngine.HideInInspector] public TMP_Dropdown dropdown;
        public List<object> properties = new List<object>();

        public override void Start()
        {
            IsActive = false;
            dropdown = GetComponent<TMP_Dropdown>();
            if (dropdown == null) return;
            Assign();
            Initialize();
            SetIndexCurrent();
            base.Start();
            IsActive = true;
        }

        private void Update()
        {
            if (NameText != null && dropdown != null) NameText.text = GetPropertyName(properties[dropdown.value]);
        }

        public abstract void Assign();

        public void SetValue(int value) => SetValue(properties[value]);
        public virtual void SetValue(object property) => SetValue(property, true);

        public virtual void Initialize()
        {
            List<TMP_Dropdown.OptionData> dropDownData = new List<TMP_Dropdown.OptionData>();
            dropdown.ClearOptions();
            for (int i = 0; i < properties.Count; i++)
                dropDownData.Add(new TMP_Dropdown.OptionData() { text = GetDropdownValueName(properties[i]) });
            dropdown.AddOptions(dropDownData);
        }

        public abstract void SetIndexCurrent();
        public virtual string GetDropdownValueName(object property) => property.ToString();
        public override string GetPropertyName(object property) => $"{OptionLabelName}{GetDropdownValueName(property)}";

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) ||
                !int.TryParse(optionValue, out int value) || dropdown == null) return;
            dropdown.value = value;
        }
    }
}