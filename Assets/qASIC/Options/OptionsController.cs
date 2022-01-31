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

        private static List<MethodInfo> _settings = new List<MethodInfo>();
        public static List<MethodInfo> Settings => _settings;

        static GameObject tempGameObject;

        public static string Path { get; private set; }
        public static SerializationType SaveType { get; private set; }

        #region Enable
        private static bool? _enabledOverride = null;
        public static bool Enabled => _enabledOverride ?? OptionsProjectSettings.Instance.enableOptionsSystem;

        public static void OverrideEnabled(bool enabled)
        {
            if (enabled == _enabledOverride) return;
            _enabledOverride = enabled;
            switch (enabled)
            {
                case true:
                    CreateSettingsList();
                    break;
                case false:
                    UserPreferences.Clear();
                    _settings.Clear();
                    DisposeTemp();
                    break;
            }
        }
        #endregion

        #region Initializing

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Initialize()
        {
            if (!Enabled) return;
            CreateSettingsList();
            LoadUserPreferences();
        }

        private static void CreateSettingsList()
        {
            IEnumerable<MethodInfo> methodInfos = TypeFinder.FindAllAttributes<OptionsSetting>();
            _settings = methodInfos.ToList();

            UserPreferences.Clear();
            foreach (MethodInfo setting in _settings)
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
        #endregion

        #region Loading
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
            if (!Enabled) return;
            SaveType = SerializationType.playerPrefs;
            Path = string.Empty;

            Dictionary<string, string> prefs = new Dictionary<string, string>(UserPreferences);
            foreach (KeyValuePair<string, string> preference in prefs)
                if (PlayerPrefs.HasKey(preference.Key))
                    ChangeOption(preference.Key, PlayerPrefs.GetString(preference.Key), false, false);

            qDebug.Log("Loaded user settings", "settings");
        }

        public static void LoadConfig(string path)
        {
            if (!Enabled) return;
            SaveType = SerializationType.config;
            Path = path;

            if (!ConfigController.TryCreateListFromFile(Path, out List<KeyValuePair<string, string>> settings)) return;

            for (int i = 0; i < settings.Count; i++)
                ChangeOption(settings[i].Key, settings[i].Value, false, false);

            qDebug.Log("Loaded user settings", "settings");
        }
        #endregion

        #region Saving
        public static void Save()
        {
            if (!Enabled) return;
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
            if (!Enabled) return;
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
        #endregion

        #region Get and set
        public static void ChangeOption(string optionName, object value, bool log = true, bool save = true)
        {
            if (!Enabled) return;
            optionName = optionName.ToLower();

            int targetsCount = 0;

            foreach (MethodInfo setting in Settings)
            {
                DisposeTemp();

                try
                {
                    if (!TryGetAttribute(setting, out OptionsSetting attr)) continue;
                    if (!TryGetSettingValueType(setting, out Type valueType)) continue;

                    object val = value;
                    if (value is string) val = Convert.ChangeType(value, valueType);

                    if ((optionName != attr.Name.ToLower() || val.GetType() != valueType) &&
                        (val.GetType() == typeof(int) || !valueType.IsEnum)) continue;

                    var obj = CreateClass(setting.DeclaringType);
                    setting.Invoke(obj, new object[] { val });

                    targetsCount++;

                    string saveSetting = val.ToString();

                    if (!UserPreferences.ContainsKey(optionName))
                        UserPreferences.Add(optionName, string.Empty);

                    UserPreferences[optionName] = saveSetting;
                }
                catch { }
            }

            DisposeTemp();

            if (log)
                LogChangeOption(optionName, value.ToString(), targetsCount);

            if (save)
                SaveSetting(optionName, value.ToString());
        }

        public static bool TryGetOptionValue(string optionName, out string value)
        {
            optionName = optionName.ToLower();
            bool exists = UserPreferences.ContainsKey(optionName);
            value = exists ? UserPreferences[optionName] : string.Empty;
            return exists;
        }
        #endregion

        #region Utility
        static void LogChangeOption(string optionName, string value, int targetsCount)
        {
            if (targetsCount == 0)
            {
                qDebug.LogError($"Couldn't find setting '{optionName}'!");
                return;
            }

            qDebug.Log($"Changed '{optionName}' to '{value}'", "settings");
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
        #endregion

        #region Temp object
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

        static void DisposeTemp()
        {
            if (tempGameObject)
                UnityObject.Destroy(tempGameObject);
        }
        #endregion
    }
}