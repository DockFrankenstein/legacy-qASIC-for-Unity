using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

namespace qASIC.Options.Menu
{
    public abstract class OptionsDropdownValueBase<T> : OptionsDropdown
    {
        [Header("Values")]
        [SerializeField] [InspectorLabel("Autofill")] bool autofillValues = true;
        [SerializeField] List<T> values = new List<T>();

        protected override void Awake()
        {
            base.Awake();

            if (autofillValues)
                AutoFillDropdown();
        }

        //Auto update in editor
        private void OnValidate()
        {
            if (!autofillValues) return;
            if (Application.isPlaying) return;
            AutoFillDropdown();
        }

        void AutoFillDropdown()
        {
            if (dropdown == null) return;
            dropdown.options = values.ConvertAll(x => new TMP_Dropdown.OptionData(x.ToString()));
        }

        public override void SetValue(object value, bool log)
        {
            if (!(value is int i))
            {
                Debug.LogError("Index is not an int!");
                return;
            }

            if (i < 0 || i >= values.Count)
            {
                Debug.LogError("Index is out of range!");
                return;
            }

            base.SetValue(values[i], log);
        }

        public override void LoadOption()
        {
            if (dropdown == null) return;
            if (!OptionsController.TryGetOptionValue(optionName, out string textValue)) return;

            T value;
            try
            {
                value = (T)Convert.ChangeType(textValue, typeof(T));
            }
            catch (Exception e)
            {
                qDebug.LogError($"Converting setting '{optionName.ToLower()}' failed: {e}");
                return;
            }

            int index = values.IndexOf(value);
            if (index == -1) return;
            dropdown.SetValueWithoutNotify(index);
        }
    }
}