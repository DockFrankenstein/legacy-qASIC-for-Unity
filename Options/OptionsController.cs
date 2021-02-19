using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using qASIC.FileManaging;

namespace qASIC.Options
{
    public static class OptionsController
    {
        private static string _config;
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
                ChangeOption(values[0], values[1], false);
            }
            Console.GameConsoleController.Log("Loaded user settings", "settings");
        }

        public static void Save()
        {
            FileManager.SaveFileWriter($"{UnityEngine.Application.persistentDataPath}/{_path}", _config);
            Console.GameConsoleController.Log($"Saved user settings preferences to {_path}", "settings");
        }

        public static void LoadSettings()
        {
            IEnumerable<MethodInfo> methodInfos = TypeFinder.FindAllAttributes<OptionsSetting>();
            _settings = methodInfos.ToList();
        }

        public static void ChangeOption(string optionName, object parameter, bool log = true)
        {
            foreach (MethodInfo setting in Settings)
            {
                var obj = Activator.CreateInstance(setting.DeclaringType);
                try
                {
                    OptionsSetting attr = (OptionsSetting)setting.GetCustomAttributes(typeof(OptionsSetting), true)[0];
                    if (parameter.GetType() != typeof(int) && attr.ValueType.IsEnum && parameter.GetType() != attr?.ValueType
                        || optionName.ToLower() != attr?.Name) continue;
                    setting.Invoke(obj, new object[] { parameter });
                    if (log) Console.GameConsoleController.Log($"Changed <b>{attr.Name}</b> to {parameter}", "settings");
                    _config = ConfigController.SetSetting(_config, attr.Name, parameter.ToString());
                }
                catch { }
            }
        }
    }
}