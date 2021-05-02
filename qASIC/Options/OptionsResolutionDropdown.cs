using UnityEngine;
using System.Collections.Generic;

namespace qASIC.Options.Menu
{
    public class OptionsResolutionDropdown : AdvancedOptionsDropdown
    {
        public override void Assign()
        {
            Resolution[] resolutions = Screen.resolutions;
            List<object> resolutionList = new List<object>();
            for (int i = 0; i < resolutions.Length; i++)
            {
                Vector2Int res = new Vector2Int(resolutions[i].width, resolutions[i].height);
                if (resolutionList.Contains(res)) continue;
                resolutionList.Add(res);
            }
            properties = resolutionList;
        }

        public override void SetValue(object propertie) =>
            base.SetValue(VectorStringConvertion.Vector2IntToString((Vector2Int)propertie));

        public override void SetCurrentIndex()
        {
            int index = properties.IndexOf(new Vector2Int(Screen.width, Screen.height));
            if (index < 0) return;
            _dropdown.SetValueWithoutNotify(index);
        }

        public override string GetDropdownValueName(object property)
        {
            if (!(property is Vector2Int)) return string.Empty;
            Vector2Int res = (Vector2Int)property;
            return VectorStringConvertion.Vector2IntToString(res);
        }

        public override void LoadOption()
        {
            if (_dropdown == null) return;
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) ||
                !VectorStringConvertion.TryStringToVector2Int(optionValue, out Vector2Int result)) return;
            _dropdown.SetValueWithoutNotify(properties.IndexOf(result));
        }
    }
}