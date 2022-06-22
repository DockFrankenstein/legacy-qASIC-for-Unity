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
        public static Dictionary<string, object> UserPreferences { get; private set; } = new Dictionary<string, object>();

        private static List<MethodInfo> _settings = new List<MethodInfo>();
        public static List<MethodInfo> Settings => _settings;

        static GameObject tempGameObject;

        public static string Path { get; private set; }
        public static SerializationType SaveType { get; private set; }

        #region Start args
        public static bool DisableLoading { get; set; }
        public static bool DisableSaving { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void LoadArgs()
        {
            OptionsProjectSettings settings = OptionsProjectSettings.Instance;
            string[] args = Environment.GetCommandLineArgs();

            DisableSaving = settings.startArgsDisableSave && Array.IndexOf(args, "-qASIC-options-disable-save") != -1;
            DisableLoading = settings.startArgsDisableLoad && Array.IndexOf(args, "-qASIC-options-disable-load") != -1;

        }
        #endregion

        #region Enable
        private static bool? _enabledOverride = null;
        public static bool Enabled => _enabledOverride ?? OptionsProjectSettings.Instance.enableOptionsSystem;

        public static void OverrideEnabled(bool enabled)
        {
            if (enabled == _enabledOverride) return;
            _enabledOverride = enabled;

            if (!enabled)
            {
                UserPreferences.Clear();
                _settings.Clear();
                DisposeTemp();
            }
        }
        #endregion

        #region Initializing
        private static bool _initialized = false;
        public static bool Initialized => _initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void AutoInitialize()
        {
            if (OptionsProjectSettings.Instance.autoInitialize)
                Initialize();
        }

        public static void Initialize(bool loadUserPreferences = true)
        {
            if (!Enabled || _initialized) return;
            _initialized = true;

            OptionsProjectSettings settings = OptionsProjectSettings.Instance;

            string[] args = Environment.GetCommandLineArgs();

            qDebug.Log($"Initializing Options System v{qASIC.Internal.Info.OptionsVersion}...", "init");

            if (settings.startArgsDisableInit && Array.IndexOf(args, "-qASIC-options-disable-initialization") != -1)
            {
                qDebug.Log("Options System initialization stopped", "init");
                return;
            }

            CreateSettingsList();
            if (loadUserPreferences)
                LoadUserPreferences();

            qDebug.Log("Options System initialization complete!", "settings");
        }

        private static void CreateSettingsList()
        {
            qDebug.Log("Creating setting list...", "init");
            IEnumerable<MethodInfo> methodInfos = TypeFinder.FindAllAttributes<OptionsSetting>();
            _settings = methodInfos.ToList();

            UserPreferences.Clear();

            List<MethodInfo> tempSettings = new List<MethodInfo>(_settings);
            foreach (MethodInfo setting in tempSettings)
            {
                try
                {
                    if (!TryGetAttribute(setting, out OptionsSetting attr)) continue;
                    if (UserPreferences.ContainsKey(attr.Name.ToLower())) continue;

                    if (!setting.IsStatic)
                    {
                        qDebug.LogError($"Setting {attr.Name.ToLower()} must be static!");
                        _settings.Remove(setting);
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(attr.defaultValueMethodName) &&
                        TryGetValueOfMethodFromName(setting, attr.enableMethodName, out bool isEnabled) &&
                        !isEnabled) return;

                    object defaultValue = attr.DefaultValue != null ? attr.DefaultValue.ToString() : null;
                    if (!string.IsNullOrWhiteSpace(attr.defaultValueMethodName) &&
                        TryGetValueOfMethodFromName(setting, attr.defaultValueMethodName, out object obj))
                            defaultValue = obj.ToString();

                    UserPreferences.Add(attr.Name.ToLower(), defaultValue);
                }
                catch { }
            }

            qDebug.Log($"Successfully finished Options System setting list creation, setting count: {UserPreferences.Count}", "init");
        }
        #endregion

        #region Loading
        static void LoadUserPreferences()
        {
            if (DisableLoading) return;

            OptionsProjectSettings settings = OptionsProjectSettings.Instance;

            switch (settings.serializationType)
            {
                case SerializationType.playerPrefs:
                    LoadPrefs();
                    break;
                case SerializationType.config:
                    LoadConfig(settings.savePath.ToString());
                    break;
                case SerializationType.none:
                    break;
                default:
                    qDebug.LogError($"Serialization type '{settings.serializationType}' is not supported by the options system!");
                    break;
            }
        }

        public static void LoadPrefs()
        {
            if (!Enabled) return;
            if (DisableLoading) return;

            SaveType = SerializationType.playerPrefs;
            Path = string.Empty;

            Dictionary<string, object> prefs = new Dictionary<string, object>(UserPreferences);
            foreach (KeyValuePair<string, object> preference in prefs)
                if (PlayerPrefs.HasKey(preference.Key))
                    ChangeOption(preference.Key, PlayerPrefs.GetString(preference.Key), false, false);

            qDebug.Log("User settings successfully loaded!", "init");
        }

        public static void LoadConfig(string path)
        {
            if (!Enabled) return;
            if (DisableLoading) return;

            SaveType = SerializationType.config;
            Path = path;

            if (!ConfigController.TryCreateListFromFile(Path, out List<KeyValuePair<string, string>> settings)) return;

            for (int i = 0; i < settings.Count; i++)
                ChangeOption(settings[i].Key, settings[i].Value, false, false);

            qDebug.Log("User settings successfully loaded!", "init");
        }
        #endregion

        #region Saving
        public static void Save()
        {
            if (!Enabled) return;
            if (DisableSaving) return;

            try
            {
                foreach (KeyValuePair<string, object> setting in UserPreferences)
                    SaveSetting(setting.Key, setting.Value.ToString());
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
            if (DisableSaving) return;

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
        /// <returns>Returns the ammount of affected settings</returns>
        public static int ChangeOption(string optionName, object value, bool log = true, bool save = true)
        {
            if (!Enabled) return 0;
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

            return targetsCount;
        }

        public static bool TryGetOptionValue(string optionName, out object value)
        {
            optionName = optionName.ToLower();
            bool exists = UserPreferences.ContainsKey(optionName);
            value = exists ? UserPreferences[optionName] : null;
            return exists;
        }

        public static bool TryGetOptionValue(string optionName, out string value)
        {
            bool exists = TryGetOptionValue(optionName, out object o);
            value = o == null ? string.Empty : o.ToString();
            return exists;
        }
        #endregion

        #region Utility
        private static bool TryGetValueOfMethodFromName<T>(MethodInfo info, string methodName, out T value)
        {
            value = default;

            object obj;

            try
            {
                MethodInfo defaultValueMethod = info.DeclaringType.GetMethod(methodName);
                obj = defaultValueMethod.Invoke(null, new object[0]);
            }
            catch
            {
                return false;
            }

            try
            {
                value = (T)Convert.ChangeType(obj, typeof(T));
                return true;
            }
            catch
            {
                Debug.LogError($"Couldn't convert return value of method '{methodName}' to '{typeof(T)}'!");
                return false;
            }
        }

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

        public static List<OptionsSetting> GetSettingsList()
        {
            int count = Settings.Count;
            List<OptionsSetting> list = new List<OptionsSetting>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    if (!TryGetAttribute(Settings[i], out OptionsSetting setting)) continue;
                    list.Add(setting);
                }
                catch { }
            }

            return list;
        }

        public static List<string> GetSettingNames()
        {
            List<OptionsSetting> SettingsList = GetSettingsList();
            List<string> namesList = new List<string>();

            for (int i = 0; i < SettingsList.Count; i++)
                namesList.Add(SettingsList[i].Name);

            return namesList;
        }
        #endregion

        #region Temp object
        static object CreateClass(Type type)
        {
            return null;

            //qASIC doesn't support non static field anymore, as they caused a lot of issues
            //and didn't make much sense (as they were still treated as static)
            //if (type.IsSubclassOf(typeof(MonoBehaviour)))
            //{
            //    tempGameObject = new GameObject("Options Setting temp script");
            //    tempGameObject.SetActive(false);
            //    return tempGameObject.AddComponent(type);
            //}
            //return Activator.CreateInstance(type);
        }

        static void DisposeTemp()
        {
            if (tempGameObject)
                UnityObject.Destroy(tempGameObject);
        }
        #endregion
    }
}