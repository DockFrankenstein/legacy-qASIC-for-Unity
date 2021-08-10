using UnityEngine;
using TMPro;

namespace qASIC.Options.Menu
{
    public abstract class MenuOption : MonoBehaviour
    {
        [Header("Updating name")]
        public TextMeshProUGUI nameText;
        public string optionLabelName;

        [Header("Options")]
        public string optionName;
        public bool save = true;

        public virtual void Start() => LoadOption();

        public abstract void LoadOption();

        public virtual void SetValue(object value, bool log) =>
            OptionsController.ChangeOption(optionName, value, log, save);

        public abstract string GetLabel();

        public virtual void Update()
        {
            if (nameText != null) 
                nameText.text = GetLabel();
        }
    }
}