using UnityEngine;
using System.Collections.Generic;

namespace qASIC.Options.Menu
{
    public class OptionsResolutionDropdown : AdvancedOptionsDropdown
    {
        public override void Start()
        {
            CreateList();
            base.Start();
            SetCurrentIndex();
        }

        public void CreateList()
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
            base.SetValue(VectorText.ToText((Vector2Int)propertie));

        public void SetCurrentIndex()
        {
            int index = properties.IndexOf(new Vector2Int(Screen.width, Screen.height));
            if (index < 0) return;
            dropdown.SetValueWithoutNotify(index);
        }

        public override string GetDropdownValueName(object property)
        {
            if (!(property is Vector2Int)) return string.Empty;
            Vector2Int res = (Vector2Int)property;
            return VectorText.ToText(res);
        }

        public override void LoadOption()
        {
            if (dropdown == null) return;
            if (!OptionsController.TryGetUserSetting(optionName, out string optionValue) ||
                !VectorText.TryToVector2Int(optionValue, out Vector2Int result)) return;
            dropdown.SetValueWithoutNotify(properties.IndexOf(result));
        }
    }
}