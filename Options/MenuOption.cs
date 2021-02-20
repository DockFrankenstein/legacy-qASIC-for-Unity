using UnityEngine;

namespace qASIC.Options.UI
{
    public abstract class MenuOption : MonoBehaviour
    {
        public string OptionName;
        public bool save = true;

        public virtual void Start() => LoadOption();

        public abstract void LoadOption();

        public void SetValue(object value, bool log) => OptionsController.ChangeOption(OptionName, value, log, save);
    }
}