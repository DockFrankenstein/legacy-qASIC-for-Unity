using System.Collections.Generic;
using UnityEngine;
using System;
using qASIC.FileManagement;
using qASIC.InputManagement.Map;
using qASIC.ProjectSettings;
using qASIC.InputManagement.Players;

namespace qASIC.InputManagement
{
    [Flags]
    public enum InputEventType
    {
        None = 0,
        Pressed = 1,
        Down = 2,
        Up = 4,
    }

    public static class InputManager
    {
        public static InputMap Map { get; private set; }
        public static bool MapLoaded => Map != null;

        #region Static
        private static GamepadButton[] _gamepadButtons = null;
        public static GamepadButton[] GamepadButtons
        {
            get
            {
                if (_gamepadButtons == null)
                    _gamepadButtons = (GamepadButton[])Enum.GetValues(typeof(GamepadButton));

                return _gamepadButtons;
            }
        }
        #endregion

        #region Saving
        private static string SavePath { get; set; }
        private static SerializationType SaveType { get; set; } = SerializationType.playerPrefs;

        public static void SaveKeys(SerializationType saveType)
        {
            if (DisableSaving) return;

            SaveType = saveType;
            SaveKeys();
        }

        public static void SaveKeys()
        {
            if (DisableSaving) return;

            //foreach (var player in UserActions)
            //{
            //    foreach (var action in player.Value)
            //    {

            //    }
            //}

            switch (SaveType)
            {
                case SerializationType.config:
                    if (string.IsNullOrWhiteSpace(SavePath))
                    {
                        qDebug.LogError("Cannot save user input preferences, path is empty!");
                        return;
                    }

                    //ConfigController.SetSettingFromFile(SavePath, saveKey, key.ToString());
                    break;
                case SerializationType.playerPrefs:
                    //PlayerPrefs.SetInt(saveKey, (int)key);
                    break;
                case SerializationType.none:
                    break;
                default:
                    qDebug.LogError($"Serialization type '{SaveType}' is not supported by the input system!");
                    break;
            }

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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
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

            for (int i = 0; i < Map.groups.Count; i++)
                Map.groups[i].CheckForRepeating();

            //UserActions.Clear();

            //foreach (InputGroup group in Map.Groups)
            //    foreach (InputAction action in group.actions)
            //        UserActions.Add(action, action.Duplicate());

            InputPlayerManager.RebuildPlayerMapData(map);

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

            //for (int i = 0; i < keys.Count; i++)
            //{
            //    string key = keys[i].GetSaveKey();
            //    if (!ConfigController.TryGettingSetting(content, key, out string setting)) continue;
            //    if (!Enum.TryParse(setting, out KeyCode result)) continue;
            //    ChangeInput(keys[i].group.groupName, keys[i], keys[i].index, result, false, false);
            //}

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

            //for (int i = 0; i < keys.Count; i++)
            //{
            //    string key = keys[i].GetSaveKey();
            //    if (!PlayerPrefs.HasKey(key)) continue;
            //    ChangeInput(keys[i].group.groupName, keys[i].action.actionName, keys[i].index, (KeyCode)PlayerPrefs.GetInt(key), false, false);
            //}

            qDebug.Log("Cablebox preferences successfully loaded!", "init");
        }
        #endregion

        #region KeyData
        public struct KeyData
        {
            public InputGroup group;
            public InputBinding item;
            public int index;

            public string GroupName { get => group.groupName; }
            public string ActionName { get => string.Empty/*action.actionName*/; }

            /// <returns>Returns key used for saving and loading using player prefs</returns>
            public string GetSaveKey()
            { /*GenerateSaveKey(group.groupName, action.actionName, index);*/ return string.Empty; }

            public static string GenerateSaveKey(string groupName, string actionName, int index) =>
                $"qASIC_Input_{groupName.ToLower()}_{actionName.ToLower()}_{index}";

            public KeyData(InputGroup group, InputBinding item, int index)
            {
                this.group = group;
                this.item = item;
                this.index = index;
            }
        }

        /// <summary>Generates a list of keys being used in the Input Manager</summary>
        public static List<KeyData> GenerateKeyList()
        {
            List<KeyData> keys = new List<KeyData>();

            if (!MapLoaded)
            {
                qDebug.LogError("Cannot load user keys, Map has not been loaded!");
                return keys;
            }

            //foreach (var player in UserActions.Values)
            //{
            //    foreach (InputAction action in player.Values)
            //    {
            //        InputAction userAction = UserActions[action];
            //        for (int i = 0; i < userAction.keys.Count; i++)
            //            keys.Add(new KeyData(group, userAction, i));
            //    }
            //}

            return keys;
        }
        #endregion

        #region Remapping
        public static void ChangeInput(string actionName, int index, KeyCode newKey, bool save = true, bool log = true) =>
            ChangeInput(MapLoaded ? Map.DefaultGroupName : string.Empty, actionName, index, newKey, save, log);

        public static void ChangeInput(string groupName, string actionName, int index, KeyCode newKey, bool save = true, bool log = true)
        {
            //if (!MapLoaded) return;
            //if (!TryGetInputAction(groupName, actionName, out InputAction action, true)) return;
            //if (!action.TryGetKey(index, out _, true)) return;

            ////UserActions[action].keys[index] = newKey;

            //if (save)
            //    SaveKey(groupName, actionName, index, newKey);

            //if (log)
            //    qDebug.Log($"Changed key {action.actionName} to {newKey}", "input");
        }
        #endregion

        #region Getting Input
        //Get input via name
        public static bool GetInputDown(string groupName, string actionName) =>
            GetInputDown(-1, groupName, actionName);
        public static bool GetInput(string groupName, string actionName) =>
            GetInput(-1, groupName, actionName);

        public static bool GetInputUp(string groupName, string actionName) =>
            GetInputUp(-1, groupName, actionName);

        public static bool GetInputDown(string actionName) =>
            GetInputDown(-1, actionName);

        public static bool GetInput(string actionName) =>
            GetInput(-1, actionName);

        public static bool GetInputUp(string actionName) =>
            GetInput(-1, actionName);

        public static bool GetInputDown(int playerIndex, string groupName, string actionName) =>
            HandleInput(playerIndex, new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }), groupName, actionName);

        public static bool GetInput(int playerIndex, string groupName, string actionName) =>
            HandleInput(playerIndex, new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKey(key); }), groupName, actionName);

        public static bool GetInputUp(int playerIndex, string groupName, string actionName) =>
            HandleInput(playerIndex, new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }), groupName, actionName);

        public static bool GetInputDown(int playerIndex, string actionName) =>
            HandleInput(playerIndex, new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }), actionName);

        public static bool GetInput(int playerIndex, string actionName) =>
            HandleInput(playerIndex, new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKey(key); }), actionName);

        public static bool GetInputUp(int playerIndex, string actionName) =>
            HandleInput(playerIndex, new Func<KeyCode, bool>((KeyCode key) => { return Input.GetKeyDown(key); }), actionName);

        public static bool HandleInput(int playerIndex, Func<KeyCode, bool> statement, string actionName) =>
            MapLoaded && HandleInput(playerIndex, statement, Map.DefaultGroupName, actionName);

        static bool HandleInput(int playerIndex, Func<KeyCode, bool> statement, string groupName, string actionName)
        {
            //if (!MapLoaded) return false;

            //if (!TryGetInputAction(groupName, actionName, out InputAction action)) return false;
            //InputAction userAction = action/*UserActions[action]*/;

            //for (int i = 0; i < userAction.keys.Count; i++)
            //{
            //    KeyCode key = userAction.keys[i];
            //    if (statement.Invoke(key))
            //        return true;
            //}

            return false;
        }

        //Get Input via reference
        public static bool GetInputUp(InputActionReference action) =>
            action != null && GetInputUp(action.GroupName, action.ActionName);

        public static bool GetInput(InputActionReference action) =>
            action != null && GetInput(action.GroupName, action.ActionName);

        public static bool GetInputDown(InputActionReference action) =>
            action != null && GetInputDown(action.GroupName, action.ActionName);

        public static bool GetInputUp(int playerIndex, InputActionReference action) =>
            action != null && GetInputUp(playerIndex, action.GroupName, action.ActionName);

        public static bool GetInput(int playerIndex, InputActionReference action) =>
            action != null && GetInput(playerIndex, action.GroupName, action.ActionName);

        public static bool GetInputDown(int playerIndex, InputActionReference action) =>
            action != null && GetInputDown(playerIndex, action.GroupName, action.ActionName);
        #endregion

        #region Get Action and Axis
        public static InputBinding GetInputAction(string groupName, string actionName)
        {
            TryGetInputAction(groupName, actionName, out InputBinding action, true);
            return action;
        }

        public static bool TryGetInputAction(string groupName, string actionName, out InputBinding action, bool logError = false)
        {
            action = null;
            //return Map.TryGetGroup(groupName, out InputGroup group, logError) && group.TryGetItem(actionName, out action, logError);
            return false;
        }

        public static Input1DAxis GetInputAxis(string groupName, string axisName)
        {
            TryGetInputAxis(groupName, axisName, out Input1DAxis axis, true);
            return axis;
        }

        public static bool TryGetInputAxis(string groupName, string axisName, out Input1DAxis axis, bool logError = false)
        {
            axis = null;
            //return Map.TryGetGroup(groupName, out InputGroup group, logError) && group.TryGetAxis(axisName, out axis, logError);
            return false;
        }
        #endregion

        #region Axis
        public static float GetMapAxis(string groupName, string axisName) =>
            GetMapAxis(-1, groupName, axisName);

        public static float GetMapAxis(string axisName) =>
            GetMapAxis(-1, axisName);

        public static float GetMapAxisRaw(string groupName, string axisName) =>
            GetMapAxisRaw(-1, groupName, axisName);

        public static float GetMapAxis(int playerIndex, string groupName, string axisName)
        {
            playerIndex = -1;
            if (!MapLoaded) return 0f;
            if (!TryGetInputAxis(groupName, axisName, out Input1DAxis axis)) return 0f;

            float value = 0f;
            if (GetInput(groupName, axis.positiveAction)) value += 1f;
            if (GetInput(groupName, axis.negativeAction)) value -= 1f;

            return value;
        }

        public static float GetMapAxis(int playerIndex, string axisName) =>
            MapLoaded ? GetMapAxis(playerIndex, Map.DefaultGroupName, axisName) : 0f;

        public static float GetMapAxisRaw(int playerIndex, string groupName, string axisName)
        {
            float value = GetMapAxis(playerIndex, groupName, axisName);
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