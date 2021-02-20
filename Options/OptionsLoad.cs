using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC.FileManaging;

namespace qASIC.Options
{
    public class OptionsLoad : MonoBehaviour
    {
        public string UserSavePath = "qASIC/Settings.txt";
        public string EditorUserSavePath = "qASIC/Setting-editor.txt";
        public TextAsset SaveFilePreset;

        private void Awake()
        {
            OptionsController.LoadSettings();
            LoadPreferences();
        }

        public void LoadPreferences()
        {
            if(SaveFilePreset != null) ConfigController.Repair(UserSavePath, SaveFilePreset.text);
            string path = UserSavePath;
#if UNITY_EDITOR
            path = EditorUserSavePath;
#endif
            OptionsController.Load(path);
        }
    }
}