using UnityEngine;

namespace qASIC.Options.Menu
{
    public class OptionsFramelimitDropdown : OptionsDropdown
    {
        public int[] values;

        public override void Assign()
        {
            properties.Clear();
            for (int i = 0; i < values.Length; i++) properties.Add(values[i]);
        }

        public override void SetIndexCurrent()
        {
            dropDown.value = properties.IndexOf(Application.targetFrameRate);
        }

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) || dropDown == null ||
                !int.TryParse(optionValue, out int result)) return;
            dropDown.value = properties.IndexOf(result);
        }
    }
}