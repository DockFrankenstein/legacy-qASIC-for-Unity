using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using qASIC.FileManagement;
using qASIC.Tools;
using UnityEngine;
using qASIC.ProjectSettings;

using UnityObject = UnityEngine.Object;

namespace qASIC.Options
{
    public static class OptionsController
    {
        public static Dictionary<string, string> UserPreferences { get; private set; } = new Dictionary<string, string>();

        private static List<MethodInfo> _settings;
        public static List<MethodInfo> Settings => _settings;

        static GameObject tempGameObject;

        public static string Path { get; private set; }
        public static SerializationType SaveType { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Initialize()
        {
            LoadSettings();
            LoadUserPreferences();
        }

        static void LoadUserPreferences()
        {
            OptionsProjectSettings settings = OptionsProjectSettings.Instance;

            switch (settings.serializationType)
            {
                case SerializationType.playerPrefs:
                    LoadPrefs();
                    break;
                case SerializationType.config:
                    LoadConfig(settings.savePath.ToString());
                    break;
                default:
                    qDebug.LogError($"Serialization type '{settings.serializationType}' is not supported by the options system!");
                    break;
            }
        }

        public static void LoadPrefs()
        {
            SaveType = SerializationType.playerPrefs;
            Path = string.Empty;

            foreach (KeyValuePair<string, string> preference in UserPreferences)
                if (PlayerPrefs.HasKey(preference.Key))
                    ChangeOption(preference.Key, PlayerPrefs.GetString(preference.Key), false, false);

            qDebug.Log("Loaded user settings", "settings");
        }

        public static void LoadConfig(string path)
        {
            SaveType = SerializationType.config;
            Path = path;

            if (!ConfigController.TryCreateListFromFile(Path, out List<KeyValuePair<string, string>> settings)) return;

            for (int i = 0; i < settings.Count; i++)
                ChangeOption(settings[i].Key, settings[i].Value, false, false);

            qDebug.Log("Loaded user settings", "settings");
        }

        public static void Save()
        {
            try
            {
                foreach (KeyValuePair<string, string> setting in UserPreferences)
                    SaveSetting(setting.Key, setting.Value);
            }
            catch (Exception e)
            {
                qDebug.LogError($"There was an error while saving user settings: {e}");
                return;
            }
            qDebug.Log("User settings successfully saved!", "settings");
        }

        static void SaveSetting(string optionName, string value)
        {
            switch (SaveType)
            {
                case SerializationType.playerPrefs:
                    PlayerPrefs.SetString(optionName, value);
                    break;
                case SerializationType.config:
                    ConfigController.SetSettingFromFile(Path, optionName, value);
                    break;
                default:
                    qDebug.LogError($"Serialization type '{SaveType}' is not supported by the options system!");
                    break;
            }
        }

        private static void LoadSettings()
        {
            IEnumerable<MethodInfo> methodInfos = TypeFinder.FindAllAttributes<OptionsSetting>();
            _settings = methodInfos.ToList();

            UserPreferences.Clear();
            foreach(MethodInfo setting in _settings)
            {
                try
                {
                    if (!TryGetAttribute(setting, out OptionsSetting attr)) continue;
                    if (UserPreferences.ContainsKey(attr.Name.ToLower())) continue;

                    string defaultValue = attr.DefaultValue != null ? attr.DefaultValue.ToString() : string.Empty;
                    if (!string.IsNullOrWhiteSpace(attr.defaultValueMethodName))
                    {
                        object classObj = CreateClass(setting.DeclaringType);
                        MethodInfo defaultValueMethod = setting.DeclaringType.GetMethod(attr.defaultValueMethodName);
                        if (defaultValueMethod.GetGenericArguments().Length == 0)
                            defaultValue = defaultValueMethod.Invoke(classObj, new object[0]).ToString();

                        DisposeTemp();
                    }

                    UserPreferences.Add(attr.Name.ToLower(), defaultValue);
                }
                catch { }
            }
        }

        public static void ChangeOption(string optionName, object parameter, bool log = true, bool save = true)
        {
            optionName = optionName.ToLower();

            int targetsCount = 0;

            foreach (MethodInfo setting in Settings)
            {
                DisposeTemp();

                try
                {
                    if (!TryGetAttribute(setting, out OptionsSetting attr)) continue;
                    if (!TryGetSettingValueType(setting, out Type valueType)) continue;

                    object param = parameter;
                    if (parameter is string) param = Convert.ChangeType(parameter, valueType);

                    if ((optionName != attr.Name.ToLower() || param.GetType() != valueType) &&
                        (param.GetType() == typeof(int) || !valueType.IsEnum)) continue;

                    var obj = CreateClass(setting.DeclaringType);
                    setting.Invoke(obj, new object[] { param });

                    targetsCount++;

                    string saveSetting = param.ToString();

                    if (!UserPreferences.ContainsKey(optionName))
                        UserPreferences.Add(optionName, string.Empty);

                    UserPreferences[optionName] = saveSetting;
                }
                catch { }
            }

            DisposeTemp();

            if (log)
            {
                qDebug.Log(targetsCount != 0 ? $"Changed '{optionName}' to '{parameter}'" : $"Couldn't find setting '{optionName}'!",
                    targetsCount != 0 ? "settings" : "error");
            }

            if (save)
                SaveSetting(optionName, parameter.ToString());
        }

        public static bool TryGetOptionValue(string optionName, out string value)
        {
            optionName = optionName.ToLower();
            bool exists = UserPreferences.ContainsKey(optionName);
            value = exists ? UserPreferences[optionName] : string.Empty;
            return exists;
        }

        static object CreateClass(Type type)
        {
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                tempGameObject = new GameObject("Options Setting temp script");
                tempGameObject.SetActive(false);
                return tempGameObject.AddComponent(type);
            }
            return Activator.CreateInstance(type);
        }

        static bool TryGetAttribute(MethodInfo method, out OptionsSetting setting)
        {
            setting = null;
            OptionsSetting[] attrs = (OptionsSetting[])method.GetCustomAttributes(typeof(OptionsSetting), true);
            if (attrs.Length != 1) return false;

            setting = attrs[0];
            return true;
        }

        static bool TryGetSettingValueType(MethodInfo method, out Type type)
        {
            type = null;
            ParameterInfo[] args = method.GetParameters();
            if (args.Length != 1) return false;

            type = args[0].ParameterType;
            return true;
        }

        static void DisposeTemp()
        {
            if (tempGameObject)
                UnityObject.Destroy(tempGameObject);
        }
    }
}