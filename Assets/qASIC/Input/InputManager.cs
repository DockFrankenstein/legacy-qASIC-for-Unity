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
        public static List<InputPlayer> Players => InputPlayerManager.Players;

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

        #region Get Input
        public static bool GetInput(string itemName) =>
            Players[0].GetInput(itemName);

        public static bool GetInputUp(string itemName) =>
            Players[0].GetInputUp(itemName);

        public static bool GetInputDown(string itemName) =>
            Players[0].GetInputDown(itemName);

        public static bool GetInput(string groupName, string itemName) =>
            Players[0].GetInput(groupName, itemName);

        public static bool GetInputUp(string groupName, string itemName) =>
            Players[0].GetInputUp(groupName, itemName);

        public static bool GetInputDown(string groupName, string itemName) =>
            Players[0].GetInputDown(groupName, itemName);
        #endregion

        #region Get Custom Item Input
        public static float GetFloatInput(string itemName) =>
            Players[0].GetFloatInput(itemName);

        public static Vector2 GetVector2Input(string itemName) =>
            Players[0].GetVector2Input(itemName);

        public static Vector3 GetVector3Input(string itemName) =>
            Players[0].GetVector3Input(itemName);

        public static float GetFloatInput(string groupName, string itemName) =>
            Players[0].GetFloatInput(groupName, itemName);

        public static Vector2 GetVector2Input(string groupName, string itemName) =>
            Players[0].GetVector2Input(groupName, itemName);

        public static Vector3 GetVector3Input(string groupName, string itemName) =>
            Players[0].GetVector3Input(groupName, itemName);
        #endregion

        #region Get Input Value
        public static object GetInputValue(string groupName, string itemName) =>
            Players[0].GetInputValue(groupName, itemName);

        public static T GetInputValue<T>(string groupName, string itemName) =>
            Players[0].GetInputValue<T>(groupName, itemName);

        public static InputEventType GetInputEvent(string itemName) =>
            Players[0].GetInputEvent(itemName);

        public static InputEventType GetInputEvent(string groupName, string itemName) =>
            Players[0].GetInputEvent(groupName, itemName);
        #endregion
    }
}