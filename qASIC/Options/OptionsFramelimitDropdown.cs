using UnityEngine;

namespace qASIC.Options.Menu
{
    public class OptionsFramelimitDropdown : AdvancedOptionsDropdown
    {
        [Space]
        [Tooltip("Toggles if value -1 should be replaced with off")]
        public bool ReplaceWithOff = true;
        public int[] Framerates;

        public override void Assign()
        {
            properties.Clear();
            for (int i = 0; i < Framerates.Length; i++) properties.Add(Framerates[i]);
        }

        public override string GetDropdownValueName(object property)
        {
            if (!(property is int)) return base.GetDropdownValueName(property);
            int value = (int)property;
            if (value == -1 && ReplaceWithOff) return "Off";
            return value.ToString();
        }

        public override void SetCurrentIndex() =>
            _dropdown.SetValueWithoutNotify(properties.IndexOf(Application.targetFrameRate));

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) || _dropdown == null ||
                !int.TryParse(optionValue, out int result)) return;

            int index = properties.IndexOf(result);
            if (index < 0) return;
            _dropdown.SetValueWithoutNotify(index);
        }
    }
}