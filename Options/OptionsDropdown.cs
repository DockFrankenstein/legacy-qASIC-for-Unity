using System.Collections.Generic;
using TMPro;

namespace qASIC.Options.Menu
{
    public abstract class OptionsDropdown : MenuOption
    {
        [UnityEngine.HideInInspector] public TMP_Dropdown dropDown;
        public List<object> properties = new List<object>();

        public override void Start()
        {
            isActive = false;
            dropDown = GetComponent<TMP_Dropdown>();
            if (dropDown == null) return;
            Assign();
            Initialize();
            SetIndexCurrent();
            base.Start();
            isActive = true;
        }

        private void Update()
        {
            if (NameText != null && dropDown != null) NameText.text = GetPropertyName(properties[dropDown.value]);
        }

        public abstract void Assign();

        public void SetValue(int value) => SetValue(properties[value]);

        public virtual void SetValue(object propertie)
        {
            SetValue(propertie, true);
        }

        public virtual void Initialize()
        {
            List<TMP_Dropdown.OptionData> dropDownData = new List<TMP_Dropdown.OptionData>();
            dropDown.ClearOptions();
            for (int i = 0; i < properties.Count; i++)
                dropDownData.Add(new TMP_Dropdown.OptionData() { text = GetDropdownValueName(properties[i]) });
            dropDown.AddOptions(dropDownData);
        }

        public abstract void SetIndexCurrent();
        public virtual string GetDropdownValueName(object property) => property.ToString();
        public override string GetPropertyName(object property) => $"{OptionLabelName}{GetDropdownValueName(property)}";

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) ||
                !int.TryParse(optionValue, out int value) || dropDown == null) return;
            dropDown.value = value;
        }
    }
}