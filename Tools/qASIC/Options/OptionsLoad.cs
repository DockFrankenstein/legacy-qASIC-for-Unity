using UnityEngine;
using qASIC.FileManagement;

namespace qASIC.Options
{
    public class OptionsLoad : MonoBehaviour
    {
        public bool loadOnce = true;
        public string userSavePath = "qASIC/Settings.txt";
        public string editorUserSavePath = "qASIC/Setting-editor.txt";
        public TextAsset saveFilePreset;

        private static bool init = false;

        private void Awake()
        {
            if(!init) OptionsController.LoadSettings();
            LoadPreferences();
            init = true;
        }

        public void LoadPreferences()
        {
            if (init && loadOnce) return;
            string path = userSavePath;
#if UNITY_EDITOR
            path = editorUserSavePath;
#endif
            if(saveFilePreset != null) ConfigController.Repair($"{Application.persistentDataPath}/{path}", saveFilePreset.text);
            OptionsController.Load($"{Application.persistentDataPath}/{path}");
        }
    }
}