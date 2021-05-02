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
        public bool Save = true;

        public virtual void Start() => LoadOption();

        public abstract void LoadOption();

        public void SetValue(object value, bool log) =>
            OptionsController.ChangeOption(OptionName, value, log, Save);

        public abstract string GetLabel();

        public virtual void Update()
        {
            if (NameText != null) 
                NameText.text = GetLabel();
        }
    }
}