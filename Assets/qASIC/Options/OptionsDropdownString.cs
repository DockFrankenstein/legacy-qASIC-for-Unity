using UnityEngine;

namespace qASIC.Options.Menu
{
    [AddComponentMenu("qASIC/Options/Dropdown String")]
    public class OptionsDropdownString : OptionsDropdown
    {
        public override void SetValue(object value, bool log)
        {
            if (dropdown == null) return;
            if (!(value is int i)) return;
            if (i < 0 || i >= dropdown.options.Count) return;

            base.SetValue(dropdown.options[i], log);
        }
    }
}