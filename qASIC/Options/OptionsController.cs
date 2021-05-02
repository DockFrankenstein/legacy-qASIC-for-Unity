using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using qASIC.FileManagement;

namespace qASIC.Options
{
    public static class OptionsController
    {
        private static string _config = string.Empty;
        private static string _path = "qASIC/Settings.txt";

        private static List<MethodInfo> _settings;
        public static List<MethodInfo> Settings
        {
            get
            {
                if (_settings == null) LoadSettings();
                return _settings;
            }
        }

        public static void Load(string path)
        {
            _path = path;
            if (!FileManager.TryLoadFileWriter(_path, out _config)) return;
            List<string> settings = ConfigController.CreateOptionList(_config);
            for (int i = 0; i < settings.Count; i++)
            {
                string[] values = settings[i].Split(':');
                if (values.Length != 2) continue;
                ChangeOption(values[0], values[1], false, false);
            }
            Console.GameConsoleController.Log("Loaded user settings", "settings");
        }

        public static bool TryGetUserSetting(string key, out string value) => ConfigController.TryGettingSetting(_config, key, out value);
        public static void Save() => FileManager.SaveFileWriter(_path, _config);

        public static void LoadSettings()
        {
            IEnumerable<MethodInfo> methodInfos = TypeFinder.FindAllAttributes<OptionsSetting>();
            _settings = methodInfos.ToList();
        }

        public static void ChangeOption(string optionName, object parameter, bool log = true, bool save = true)
        {
            foreach (MethodInfo setting in Settings)
            {
                var obj = Activator.CreateInstance(setting.DeclaringType);
                try
                {
                    OptionsSetting attr = (OptionsSetting)setting.GetCustomAttributes(typeof(OptionsSetting), true)[0];

                    object param = parameter;
                    if (parameter is string) param = Convert.ChangeType(parameter, attr?.ValueType);

                    if ((optionName.ToLower() != attr?.Name.ToLower() || param.GetType() != attr?.ValueType) &&
                        (param.GetType() == typeof(int) || !attr.ValueType.IsEnum)) continue;

                    setting.Invoke(obj, new object[] { param });

                    if (log) Console.GameConsoleController.Log($"Changed <b>{attr.Name}</b> to {param}", "settings");

                    string saveSetting = param.ToString();
                    _config = ConfigController.SetSetting(_config, attr.Name, saveSetting);
                }
                catch { }
            }
            if (save) Save();
        }
    }
}