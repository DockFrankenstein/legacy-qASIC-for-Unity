using UnityEngine;
using System.Collections.Generic;

namespace qASIC.Options.Menu
{
    public class OptionsResolutionDropdown : OptionsDropdown
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

        public override void SetValue(object propertie)
        {
            base.SetValue(VectorStringConvertion.Vector2IntToString((Vector2Int)propertie));
        }

        public override void SetIndexCurrent()
        {
            dropDown.value = properties.IndexOf(new Vector2Int(Screen.width, Screen.height));
        }

        public override string GetDropdownValueName(object property)
        {
            if (!(property is Vector2Int)) return string.Empty;
            Vector2Int res = (Vector2Int)property;
            return $"{res.x}x{res.y}";
        }

        public override void LoadOption()
        {
            if (!OptionsController.TryGetUserSetting(OptionName, out string optionValue) || dropDown == null ||
                !VectorStringConvertion.TryStringToVector2Int(optionValue, out Vector2Int result)) return;
            dropDown.value = properties.IndexOf(result);
        }
    }
}