using UnityEngine;
using TMPro;

namespace qASIC.Options.UI
{
    public abstract class MenuOption : MonoBehaviour
    {
        [Header("Updating name")]
        public TextMeshProUGUI NameText;
        public string OptionLabelName;

        [Header("Options")]
        public string OptionName;
        public bool save = true;

        public virtual void Start() => LoadOption();

        public abstract void LoadOption();

        public void SetValue(object value, bool log) => OptionsController.ChangeOption(OptionName, value, log, save);

        public virtual string GetPropertyName(object property)
        {
            return $"{OptionLabelName}{property}";
        }
    }
}