using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using qASIC.FileManagement;
using qASIC.Tools;

namespace qASIC.Options
{
    public static class OptionsController
    {
        private static string config = string.Empty;
        private static string path = "qASIC/Settings.txt";

        private static List<MethodInfo> settings;
        public static List<MethodInfo> Settings
        {
            get
            {
                if (settings == null) LoadSettings();
                return settings;
            }
        }

        public static void Load(string path)
        {
            OptionsController.path = path;
            if (!FileManager.TryLoadFileWriter(OptionsController.path, out config)) return;
            List<string> settings = ConfigController.CreateOptionList(config);
            for (int i = 0; i < settings.Count; i++)
            {
                string[] values = settings[i].Split(':');
                if (values.Length != 2) continue;
                ChangeOption(values[0], values[1], false, false);
            }
            Console.GameConsoleController.Log("Loaded user settings", "settings");
        }

        public static bool TryGetUserSetting(string key, out string value) => ConfigController.TryGettingSetting(config, key, out value);
        public static void Save() => FileManager.SaveFileWriter(path, config);

        public static void LoadSettings()
        {
            IEnumerable<MethodInfo> methodInfos = TypeFinder.FindAllAttributes<OptionsSetting>();
            settings = methodInfos.ToList();
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
                    if (parameter is string) param = Convert.ChangeType(parameter, attr?.type);

                    if ((optionName.ToLower() != attr?.name.ToLower() || param.GetType() != attr?.type) &&
                        (param.GetType() == typeof(int) || !attr.type.IsEnum)) continue;

                    setting.Invoke(obj, new object[] { param });

                    if (log) Console.GameConsoleController.Log($"Changed <b>{attr.name}</b> to {param}", "settings");

                    string saveSetting = param.ToString();
                    config = ConfigController.SetSetting(config, attr.name, saveSetting);
                }
                catch { }
            }
            if (save) Save();
        }
    }
}