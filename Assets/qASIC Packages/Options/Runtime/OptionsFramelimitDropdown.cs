﻿using UnityEngine;

namespace qASIC.SettingsSystem.Menu
{
    [AddComponentMenu("qASIC/Options/Framelimit Dropdown")]
    public class OptionsFramelimitDropdown : AdvancedOptionsDropdown
    {
        [Space]
        [Tooltip("Toggles if value -1 should be replaced with off")]
        public bool replaceWithOff = true;
        public int[] framerates;

        protected override void Start()
        {
            CreateList();
            base.Start();
            SetCurrentIndex();
        }

        public void CreateList()
        {
            properties.Clear();
            for (int i = 0; i < framerates.Length; i++) properties.Add(framerates[i]);
        }

        public override string GetDropdownValueName(object property)
        {
            if (!(property is int)) return base.GetDropdownValueName(property);
            int value = (int)property;
            if (value == -1 && replaceWithOff) return "Off";
            return value.ToString();
        }

        public void SetCurrentIndex() =>
            dropdown.SetValueWithoutNotify(properties.IndexOf(Application.targetFrameRate));

        public override void LoadOption()
        {
            if (!OptionsController.TryGetOptionValue(optionName, out string optionValue) || dropdown == null ||
                !int.TryParse(optionValue, out int result)) return;

            int index = properties.IndexOf(result);
            if (index < 0) return;
            dropdown.SetValueWithoutNotify(index);
        }
    }
}