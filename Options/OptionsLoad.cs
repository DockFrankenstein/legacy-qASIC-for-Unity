using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC.FileManaging;

namespace qASIC.Options
{
    public class OptionsLoad : MonoBehaviour
    {
        public bool LoadOnce = true;
        public string UserSavePath = "qASIC/Settings.txt";
        public string EditorUserSavePath = "qASIC/Setting-editor.txt";
        public TextAsset SaveFilePreset;

        private static bool _init = false;

        private void Awake()
        {
            if(!_init) OptionsController.LoadSettings();
            LoadPreferences();
            _init = true;
        }

        public void LoadPreferences()
        {
            if (_init && LoadOnce) return;
            if(SaveFilePreset != null) ConfigController.Repair(UserSavePath, SaveFilePreset.text);
            string path = UserSavePath;
#if UNITY_EDITOR
            path = EditorUserSavePath;
#endif
            OptionsController.Load(path);
        }
    }
}