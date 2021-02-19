using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC.FileManaging;

namespace qASIC.Options
{
    public class OptionsLoad : MonoBehaviour
    {
        public string UserSavePath = "qASIC/Settings.txt";
        public TextAsset SaveFilePreset;

        private void Awake()
        {
            OptionsController.LoadSettings();
            LoadPreferences();
        }

        public void LoadPreferences()
        {
            if(SaveFilePreset != null) ConfigController.Repair(UserSavePath, SaveFilePreset.text);
            OptionsController.Load(UserSavePath);
        }
    }
}