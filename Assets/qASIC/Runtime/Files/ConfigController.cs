using System.Collections.Generic;

namespace qASIC.FileManagement
{
    public static class ConfigController
    {
        #region GetSetting
        public static string GetSettingFromFile(string path, string key)
        {
            if (TryGettingSettingFromFile(path, key, out string setting))
                throw new System.Exception("Couldn't get setting from file: setting or file does not exist!");

            return setting;
        }

        public static string GetSetting(string content, string key)
        {
            if (TryGettingSetting(content, key, out string setting))
                throw new System.Exception("Couldn't get setting: setting does not exist!");

            return setting;
        }

        public static bool TryGettingSettingFromFile(string path, string key, out string setting)
        {
            setting = string.Empty;
            return FileManager.TryLoadFileWriter(path, out string content)
                && TryGettingSetting(content, key, out setting);
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
        [System.Obsolete]
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

        [System.Obsolete]
        public static List<string> CreateOptionList(string content)
        {
            List<string> stringList = new List<string>();
            List<KeyValuePair<string, string>> list = CreateList(content);

            for (int i = 0; i < list.Count; i++)
                stringList.Add($"{list[i].Key}:{list[i].Value}");

            return stringList;
        }

        public static List<KeyValuePair<string, string>> CreateList(string content)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            string[] lines = content.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#")) continue;
                string[] values = lines[i].Split(new string[] { ": " }, System.StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 2) continue;
                list.Add(new KeyValuePair<string, string>(values[0], values[1]));
            }

            return list;
        }

        public static bool TryCreateListFromFile(string path, out List<KeyValuePair<string, string>> list)
        {
            list = null;

            if (!FileManager.TryLoadFileWriter(path, out string content))
                return false;

            list = CreateList(content);
            return true;
        }

        public static List<KeyValuePair<string, string>> CreateListFromFile(string path)
        {
            if (!FileManager.TryLoadFileWriter(path, out string content))
                throw new System.Exception("Couldn't generate config list from file: file does not exist!");

            return CreateList(content);
        }

        public static Dictionary<string, string> CreateDictionary(string content)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            List<KeyValuePair<string, string>> list = CreateList(content);

            for (int i = 0; i < list.Count; i++)
                if (!dictionary.ContainsKey(list[i].Key))
                    dictionary.Add(list[i].Key, list[i].Value);

            return dictionary;
        }

        public static bool TryCreateDictionaryFromFile(string path, out Dictionary<string, string> dictionary)
        {
            dictionary = null;

            if (!FileManager.TryLoadFileWriter(path, out string content)) return false;
            dictionary = CreateDictionary(content);
            return true;
        }

        public static Dictionary<string, string> CreateDictionaryFromFile(string path)
        {
            if (!TryCreateDictionaryFromFile(path, out Dictionary<string, string> dictionary))
                throw new System.Exception("Couldn't generate config dictionary from file: file does not exist!");

            return dictionary;
        }
    }
}