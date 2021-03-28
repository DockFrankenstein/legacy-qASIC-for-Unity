using UnityEngine;

namespace qASIC.Options.Menu
{
    public class OptionsFramelimitDropdown : OptionsDropdown
    {
        public int[] Values;

        public override void Assign()
        {
            properties.Clear();
            for (int i = 0; i < Values.Length; i++) properties.Add(Values[i]);
        }

        public override string GetDropdownValueName(object property)
        {
            if (!(property is int)) return base.GetDropdownValueName(property);
            int value = (int)property;
            if (value == -1) return "Off";
            return value.ToString();
        }

        public override void SetIndexCurrent()
        {
            dropdown.value = properties.IndexOf(Application.targetFrameRate);
        }

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) || dropdown == null ||
                !int.TryParse(optionValue, out int result)) return;
            dropdown.value = properties.IndexOf(result);
        }
    }
}