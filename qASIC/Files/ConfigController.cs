using System.Collections.Generic;

namespace qASIC.FileManagement
{
    public static class ConfigController
    {
        #region GetSetting
        public static string GetSettingFromFile(string path, string key)
        {
            if (TryGettingSettingFromFile(path, key, out string setting)) throw new System.Exception("Couldn't get setting from file: setting or file doesn't exist!");
            return setting;
        }

        public static string GetSetting(string content, string key)
        {
            if(TryGettingSetting(content, key, out string setting)) throw new System.Exception("Couldn't get setting: setting doesn't exist!");
            return setting;
        }

        public static bool TryGettingSettingFromFile(string path, string key, out string setting)
        {
            setting = string.Empty;
            return FileManager.TryLoadFileWriter(path, out string content) && TryGettingSetting(content, key, out setting);
        }

        public static bool TryGettingSetting(string content, string key, out string setting)
        {
            string[] settings = content.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i].StartsWith("#")) continue;
                string[] values = settings[i].Split(new string[] { ": " }, System.StringSplitOptions.RemoveEmptyEntries);
                if (values.Length == 2 && values[0] == key)
                {
                    setting = values[1];
                    return true;
                }
            }
            setting = string.Empty;
            return false;
        }
        #endregion

        #region SetOption
        public static string SetSetting(string content, string key, string setting)
        {
            bool exists = false;
            string[] settings = new string[0];
            if (content.Length != 0) settings = content.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i].StartsWith("#")) continue;
                string[] values = settings[i].Split(new string[] { ": " }, System.StringSplitOptions.RemoveEmptyEntries);
                if ((values.Length == 2 || values.Length == 1) && values[0] == key)
                {
                    exists = true;
                    settings[i] = $"{values[0]}: {setting}";
                    break;
                }
            }

            if (!exists)
            {
                System.Array.Resize(ref settings, settings.Length + 1);
                settings[settings.Length - 1] = $"{key}: {setting}";
            }
            return string.Join("\n", settings);
        }

        public static void SetSettingFromFile(string path, string key, string setting)
        {
            if (!FileManager.TryLoadFileWriter(path, out string content)) content = "";
            FileManager.SaveFileWriter(path, SetSetting(content, key, setting));
        }
        #endregion

        /// <summary>Creates a file according to the template</summary>
        public static void Repair(string path, string template)
        {
            if (!FileManager.TryLoadFileWriter(path, out string content)) return;
            string[] settings = template.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i].StartsWith("#") || string.IsNullOrWhiteSpace(settings[i])) continue;
                string[] values = settings[i].Split(new string[] { ": " }, System.StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 2) continue;
                bool exists = TryGettingSetting(content, values[0], out string setting);
                settings[i] = $"{values[0]}: {(exists ? setting : values[1])}";
            }
            FileManager.SaveFileWriter(path, string.Join("\n", settings));
        }

        public static List<string> CreateOptionList(string content)
        {
            List<string> optionsList = new List<string>();
            string[] settings = content.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i].StartsWith("#")) continue;
                string[] values = settings[i].Split(new string[] { ": " }, System.StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 2) continue;
                optionsList.Add($"{values[0]}:{values[1]}");
            }
            return optionsList;
        }
    }
}