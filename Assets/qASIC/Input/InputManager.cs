using System.Collections.Generic;
using UnityEngine;
using System;
using qASIC.FileManagement;
using qASIC.InputManagement.Map;
using qASIC.ProjectSettings;

namespace qASIC.InputManagement
{
    public static class InputManager
    {
        public static InputMap Map { get; set; }
        public static bool MapLoaded { get => Map != null; }

        private static string SavePath { get; set; }
        private static SerializationType SaveType { get; set; } = SerializationType.playerPrefs;
        private static Dictionary<string, Dictionary<string, Dictionary<int, KeyCode>>> UserKeys { get; set; } = new Dictionary<string, Dictionary<string, Dictionary<int, KeyCode>>>();

        #region Saving
        public static void SaveKeys(SerializationType saveType)
        {
            if (DisableSaving) return;

            SaveType = saveType;
            SaveKeys();
        }

        public static void SaveKeys()
        {
            if (DisableSaving) return;

            List<KeyData> keys = GenerateKeyList();

            for (int i = 0; i < keys.Count; i++)
                if (UserKeyExists(keys[i].group.groupName, keys[i].action.actionName, keys[i].index))
                    SaveKey(keys[i].GroupName, keys[i].ActionName, keys[i].index, UserKeys[keys[i].GroupName][keys[i].ActionName][keys[i].index]);

            qDebug.Log("Successfully saved user input preferences.", "input");
        }

        public static void SaveKey(string groupName, string actionName, int keyIndex, KeyCode key)
        {
            if (DisableSaving) return;

            string saveKey = KeyData.GenerateSaveKey(groupName, actionName, keyIndex);

            switch (SaveType)
            {
                case SerializationType.config:
                    if (string.IsNullOrWhiteSpace(SavePath))
                    {
                        qDebug.LogError("Cannot save key preference, path is empty!");
                        return;
                    }

                    ConfigController.SetSettingFromFile(SavePath, saveKey, key.ToString());
                    break;
                case SerializationType.playerPrefs:
                    PlayerPrefs.SetInt(saveKey, (int)key);
                    break;
                case SerializationType.none:
                    break;
                default:
                    qDebug.LogError($"Serialization type '{SaveType}' is not supported by the input system!");
                    break;
            }
        }
        #endregion

        #region Starting arguments
        public static bool DisableLoading { get; set; }
        public static bool DisableSaving { get; set; }

        static void LoadStartingArguments()
        {
            string[] args = Environment.GetCommandLineArgs();

            DisableLoading = Array.IndexOf(args, "-qASIC-input-disable-load") != -1;
            DisableSaving = Array.IndexOf(args, "-qASIC-input-disable-save") != -1;
        }
        #endregion

        #region Loading
        private static bool _initialized = false;
        public static bool Initialized => _initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            LoadStartingArguments();
            InputProjectSettings settings = InputProjectSettings.Instance;

            if (settings.map == null) return;

            qDebug.Log($"Initializing Cablebox Input System v{qASIC.Internal.Info.InputVersion}...", "init");
            LoadMap(settings.map);

            switch (settings.serializationType)
            {
                case SerializationType.playerPrefs:
                    LoadUserKeysPrefs();
                    break;
                case SerializationType.config:
                    LoadUserKeysConfig(settings.filePath.GetFullPath());
                    break;
                default:
                    qDebug.LogError($"Serialization type '{settings.serializationType}' is not supported by the input system!");
                    break;
            }

            qDebug.Log($"Cablebox initialization complete!", "input");
        }

        public static void LoadMap(InputMap map)
        {
            Map = map;

            if (Map == null) return;

            Map.CheckForRepeating();

            for (int i = 0; i < Map.Groups.Count; i++)
                Map.Groups[i].CheckForRepeating();

            UserKeys.Clear();

            qDebug.Log("Input map has been assigned", "input");
        }

        /// <summary>Loads user key preferences using Config Controller</summary>
        public static void LoadUserKeysConfig(string path)
        {
            if (DisableLoading) return;

            SaveType = SerializationType.config;
            SavePath = path;

            if (!MapLoaded)
            {
                qDebug.LogError("Cannot load user keys, Map has not been loaded!");
                return;
            }

            if (!FileManager.TryLoadFileWriter(path, out string content)) return;

            List<KeyData> keys = GenerateKeyList();

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i].GetSaveKey();
                if (!ConfigController.TryGettingSetting(content, key, out string setting)) continue;
                if (!Enum.TryParse(setting, out KeyCode result)) continue;
                ChangeInput(keys[i].group.groupName, keys[i].action.actionName, keys[i].index, result, false, false);
            }

            qDebug.Log("Cablebox preferences successfully loaded!", "init");
        }

        /// <summary>Loads user key preferences using Player Prefs</summary>
        public static void LoadUserKeysPrefs()
        {
            if (DisableLoading) return;

            SaveType = SerializationType.playerPrefs;

            if (!MapLoaded)
            {
                qDebug.LogError("Cannot load user keys, Map has not been loaded!");
                return;
            }

            List<KeyData> keys = GenerateKeyList();

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i].GetSaveKey();
                if (!PlayerPrefs.HasKey(key)) continue;
                ChangeInput(keys[i].group.groupName, keys[i].action.actionName, keys[i].index, (KeyCode)PlayerPrefs.GetInt(key), false, false);
            }

            qDebug.Log("Cablebox preferences successfully loaded!", "init");
        }
        #endregion

        #region KeyData
        public struct KeyData
        {
            public InputGroup group;
            public InputAction action;
            public int index;

            public string GroupName { get => group.groupName; }
            public string ActionName { get => action.actionName; }

            /// <returns>Returns key used for saving and loading</returns>
            public string GetSaveKey() =>
                GenerateSaveKey(group.groupName, action.actionName, index);

            public static string GenerateSaveKey(string groupName, string actionName, int index) =>
                $"qASIC_Input_{groupName.ToLower()}_{actionName.ToLower()}_{index}";

            public KeyData(InputGroup group, InputAction action, int index)
            {
                this.group = group;
                this.action = action;
                this.index = index;
            }
        }

        public static List<KeyData> GenerateKeyList()
        {
            List<KeyData> keys = new List<KeyData>();

            if (!MapLoaded)
            {
                qDebug.LogError("Cannot load user keys, Map has not been loaded!");
                return keys;
            }

            for (int groupIndex = 0; groupIndex < Map.Groups.Count; groupIndex++)
            {
                InputGroup group = Map.Groups[groupIndex];
                for (int actionIndex = 0; actionIndex < group.actions.Count; actionIndex++)
                {
                    InputAction action = group.actions[actionIndex];
                    for (int i = 0; i < action.keys.Count; i++)
                        keys.Add(new KeyData(group, action, i));
                }
            }

            return keys;
        }
        #endregion

        #region Remapping
        public static void ChangeInput(string actionName, int index, KeyCode newKey, bool save = true, bool log = true) =>
            ChangeInput(MapLoaded ? Map.DefaultGroupName : string.Empty, actionName, index, newKey, save, log);

        public static void ChangeInput(string groupName, string actionName, int index, KeyCode newKey, bool save = true, bool log = true)
        {
            if (!MapLoaded) return;
            if (!TryGetInputAction(groupName, actionName, out InputAction action, true)) return;
            if (!action.TryGetKey(index, out _, true)) return;

            CreateUserKey(groupName, actionName, index);
            UserKeys[groupName][actionName][index] = newKey;

            if (save)
                SaveKey(groupName, actionName, index, newKey);

            if (log)
                qDebug.Log($"Changed key {action.actionName} to {newKey}", "input");
        }

        static void CreateUserKey(string groupName, string keyName, int index)
        {
            if (!UserKeys.ContainsKey(groupName))
                UserKeys.Add(groupName, new Dictionary<string, Dictionary<int, KeyCode>>());

            if (!UserKeys[groupName].ContainsKey(keyName))
                UserKeys[groupName].Add(keyName, new Dictionary<int, KeyCode>());

            if (!UserKeys[groupName][keyName].ContainsKey(index))
                UserKeys[groupName][keyName].Add(index, KeyCode.None);
        }
        #endregion

        #region Get KeyCode
        public static KeyCode GetKeyCode(string actionName, int index) =>
            GetKeyCode(MapLoaded ? Map.DefaultGroupName : string.Empty, actionName, index);

        public static KeyCode GetKeyCode(string groupName, string actionName, int index)
        {
            TryGetKeyCode(groupName, actionName, index, out KeyCode key, true);
            return key;
        }

        public static bool TryGetKeyCode(string actionName, int index, out KeyCode key, bool logError) =>
            TryGetKeyCode(MapLoaded ? Map.DefaultGroupName : string.Empty, actionName, index, out key, logError);

        public static bool TryGetKeyCode(string groupName, string actionName, int index, out KeyCode key, bool logError = false)
        {
            key = KeyCode.None;

            if (!MapLoaded)
                return false;

            if (!TryGetInputAction(groupName, actionName, out InputAction action, logError) || !action.TryGetKey(index, out key, true))
                return false;

            if (UserKeyExists(groupName, actionName, index))
                key = UserKeys[groupName][actionName][index];

            return true;
        }
        #endregion

        #region Getting Input
        public static bool GetInputDown(string groupName, string actionName) =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }), groupName, actionName);

        public static bool GetInput(string groupName, string actionName) =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKey(key); }), groupName, actionName);

        public static bool GetInputUp(string groupName, string actionName) =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }), groupName, actionName);

        public static bool GetInputDown(string actionName) =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }), actionName);

        public static bool GetInput(string actionName) =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKey(key); }), actionName);

        public static bool GetInputUp(string actionName) =>
            HandleInput(new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }), actionName);

        public static bool HandleInput(Func<KeyCode, bool> statement, string actionName) =>
            MapLoaded && HandleInput(statement, Map.DefaultGroupName, actionName);

        static bool HandleInput(Func<KeyCode, bool> statement, string groupName, string actionName)
        {
            if (!MapLoaded) return false;

            if (!TryGetInputAction(groupName, actionName, out InputAction action)) return false;

            for (int i = 0; i < action.keys.Count; i++)
            {
                KeyCode key = action.keys[i];
                if (UserKeyExists(groupName, actionName, i))
                    key = UserKeys[groupName][actionName][i];

                if (statement.Invoke(key))
                    return true;
            }

            return false;
        }

        public static bool GetInputUp(InputActionReference action) =>
            action != null && GetInputUp(action.GroupName, action.ActionName);

        public static bool GetInput(InputActionReference action) =>
            action != null && GetInput(action.GroupName, action.ActionName);

        public static bool GetInputDown(InputActionReference action) =>
            action != null && GetInputDown(action.GroupName, action.ActionName);
        #endregion

        #region UserKeys Exist
        public static bool UserGroupExists(string groupName) =>
            UserKeys.ContainsKey(groupName);

        public static bool UserActionExists(string groupName, string actionName) =>
            UserGroupExists(groupName) && UserKeys[groupName].ContainsKey(actionName);

        public static bool UserKeyExists(string groupName, string actionName, int index) =>
            UserActionExists(groupName, actionName) && UserKeys[groupName][actionName].ContainsKey(index);
        #endregion

        #region Get Action and Axis
        public static InputAction GetInputAction(string groupName, string actionName)
        {
            TryGetInputAction(groupName, actionName, out InputAction action, true);
            return action;
        }

        public static bool TryGetInputAction(string groupName, string actionName, out InputAction action, bool logError = false)
        {
            action = null;
            return Map.TryGetGroup(groupName, out InputGroup group, logError) && group.TryGetAction(actionName, out action, logError);
        }

        public static InputAxis GetInputAxis(string groupName, string axisName)
        {
            TryGetInputAxis(groupName, axisName, out InputAxis axis, true);
            return axis;
        }

        public static bool TryGetInputAxis(string groupName, string axisName, out InputAxis axis, bool logError = false)
        {
            axis = null;
            return Map.TryGetGroup(groupName, out InputGroup group, logError) && group.TryGetAxis(axisName, out axis, logError);
        }
        #endregion

        #region Axis
        public static float GetMapAxis(string groupName, string axisName)
        {
            if (!MapLoaded) return 0f;
            if (!TryGetInputAxis(groupName, axisName, out InputAxis axis)) return 0f;

            float value = 0f;
            if (GetInput(groupName, axis.positiveAction)) value += 1f;
            if (GetInput(groupName, axis.negativeAction)) value -= 1f;

            return value;
        }

        public static float GetMapAxis(string axisName) =>
            MapLoaded ? GetMapAxis(Map.DefaultGroupName, axisName) : 0f;

        public static float GetMapAxisRaw(string groupName, string axisName)
        {
            float value = GetMapAxis(groupName, axisName);
            //TODO: this is stupid
            return Mathf.Round(Mathf.Abs(value)) * (value < 0 ? -1f : 1f);
        }

        public static float GetMapAxisRaw(string axisName) =>
            MapLoaded ? GetMapAxisRaw(Map.DefaultGroupName, axisName) : 0f;

        public static float GetMapAxis(InputAxisReference axis) =>
            axis == null ? 0f : GetMapAxis(axis.GroupName, axis.AxisName);

        public static float GetMapAxisRaw(InputAxisReference axis) =>
            axis == null ? 0f : GetMapAxisRaw(axis.GroupName, axis.AxisName);


        [Obsolete("GetAxis is outdated. Use GetMapAxis or CreateAxis instead.")]
        public static float GetAxis(string positive, string negative) =>
            CreateAxis(positive, negative);

        public static float CreateAxis(string positive, string negative) =>
            CreateAxis(GetInput(positive), GetInput(negative));

        public static float CreateAxis(KeyCode positive, KeyCode negative) =>
            CreateAxis(Input.GetKey(positive), Input.GetKey(negative));

        public static float CreateAxis(bool positive, bool negative)
        {
            float value = 0f;
            if (positive) value++;
            if (negative) value--;
            return value;
        }
        #endregion
    }
}