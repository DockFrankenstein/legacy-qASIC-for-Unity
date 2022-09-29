﻿using System.Collections.Generic;
using UnityEngine;
using System;
using qASIC.FileManagement;
using qASIC.InputManagement.Map;
using qASIC.ProjectSettings;
using qASIC.InputManagement.Players;
using UnityEditor.Graphs;

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
        public static string SavePath { get; set; }
        private static SerializationType SaveType { get; set; } = SerializationType.playerPrefs;

        public static void SavePreferences()
        {
            if (DisableSaving) return;
            
            FileManager.SaveFileJSON(SavePath, Players[0].MapData, true);
            qDebug.Log("[Cablebox] Player preferences saved", "input");
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

            SavePath = settings.filePath.GetFullPath();
            if (settings.map == null) return;

            qDebug.Log($"Initializing Cablebox Input System v{qASIC.Internal.Info.InputVersion}...", "init");
            LoadMap(settings.map);


            LoadPreferences();

            //switch (settings.serializationType)
            //{
            //    case SerializationType.playerPrefs:
            //        LoadUserKeysPrefs();
            //        break;
            //    case SerializationType.config:
            //        LoadUserKeysConfig(settings.filePath.GetFullPath());
            //        break;
            //    default:
            //        qDebug.LogError($"Serialization type '{settings.serializationType}' is not supported by the input system!");
            //        break;
            //}

            qDebug.Log($"Cablebox initialization complete!", "input");
        }

        public static void LoadMap(InputMap map)
        {
            Map = map;

            if (Map == null)
            {
                InputPlayerManager.RebuildPlayerMapData(null);
                return;
            }

            Map.CheckForRepeating();

            for (int i = 0; i < Map.groups.Count; i++)
                Map.groups[i].CheckForRepeating();

            InputPlayerManager.RebuildPlayerMapData(map);

            qDebug.Log("Input map has been assigned", "input");
        }

        /// <summary>Loads user key preferences using Config Controller</summary>
        public static void LoadPreferences()
        {
            InputMapData saveData = new InputMapData();
            if (!FileManager.TryReadFileJSON(SavePath, saveData))
                return;

            Players[0].MapData.LoadFromData(saveData);
            qDebug.Log("[Cablebox] Player preferences have been loaded", "input");
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
        public static void ChangeInput(string itemName, int index, string key, bool save = true, bool log = true) =>
            Players[0].ChangeInput(itemName, index, key, save, log);

        public static void ChangeInput(string groupName, string itemName, int index, string key, bool save = true, bool log = true) =>
            Players[0].ChangeInput(groupName, itemName, index, key, save, log);
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