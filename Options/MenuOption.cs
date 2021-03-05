using UnityEngine;
using TMPro;

namespace qASIC.Options.Menu
{
    public abstract class MenuOption : MonoBehaviour
    {
        [Header("Updating name")]
        public TextMeshProUGUI NameText;
        public string OptionLabelName;

        [Header("Options")]
        public string OptionName;
        public bool save = true;

        public bool isActive { get; set; }

        public virtual void Start() => LoadOption();

        public abstract void LoadOption();

        public void SetValue(object value, bool log)
        {
            if (!isActive) return;
            OptionsController.ChangeOption(OptionName, value, log, save);
        }

        public virtual string GetPropertyName(object property) => $"{OptionLabelName}{property}";
    }
}