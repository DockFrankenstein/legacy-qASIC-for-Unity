﻿using UnityEngine;

namespace qASIC.SettingsSystem.Menu
{
    [AddComponentMenu("qASIC/Options/Toggle")]
    public class OptionsToggle : MenuOption
    {
        private UnityEngine.UI.Toggle toggle;

        public bool invertEvent;
        public UnityEventBool OnValueChange = new UnityEventBool();

        private void Awake()
        {
            toggle = GetComponent<UnityEngine.UI.Toggle>();
            if (toggle != null) toggle.onValueChanged.AddListener(SetValue);
        }

        public void SetValue(bool state) 
        { 
            SetValue(state, true); 
            OnValueChange.Invoke(state != invertEvent); 
        }

        public override string GetLabel()
        {
            if (toggle == null) return string.Empty;
            return $"{optionLabelName}{toggle.isOn}";
        }

        public override void LoadOption()
        {
            if (!OptionsController.TryGetOptionValue(optionName, out string optionValue) ||
                !bool.TryParse(optionValue, out bool value) || toggle == null) return;
            toggle.SetIsOnWithoutNotify(value);
        }
    }
}