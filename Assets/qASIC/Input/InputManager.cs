using System.Collections.Generic;
using UnityEngine;
using System;

namespace qASIC.InputManagement
{
    public static class InputManager
    {
        public static InputMap Map { get; set; }
        public static bool MapLoaded { get => Map != null; }
        private static Dictionary<string, Dictionary<string, Dictionary<int, KeyCode>>> UserKeys { get; set; } = new Dictionary<string, Dictionary<string, Dictionary<int, KeyCode>>>();

        public static void SaveKeys()
        {
            //string path = $"{Application.persistentDataPath}/{GlobalKeys.SavePath}";
            //foreach (var entry in GlobalKeys.Presets)
            //    FileManagement.ConfigController.SetSettingFromFile(path, entry.Key, entry.Value.ToString());
        }

        public static void LoadMap(InputMap map)
        {
            Map = map;
            Map.CheckForRepeating();

            for (int i = 0; i < Map.Groups.Count; i++)
                Map.Groups[i].CheckForRepeating();

            qDebug.Log("Input map has been assigned", "input");
        }

        public static void LoadUserKeys()
        {
            //string path = $"{Application.persistentDataPath}/{GlobalKeys.SavePath}";
            //if (!FileManagement.FileManager.TryLoadFileWriter(path, out string content)) return;

            //List<string> settings = FileManagement.ConfigController.CreateOptionList(content);

            //for (int i = 0; i < settings.Count; i++)
            //{
            //    if (settings[i].StartsWith("#")) continue;
            //    string[] values = settings[i].Split(':');
            //    if (values.Length != 2) continue;
            //    if (GlobalKeys.Presets.ContainsKey(values[0]) && System.Enum.TryParse(values[1], out KeyCode result)) GlobalKeys.Presets[values[0]] = result;
            //}
        }

        #region Input handling
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
        #endregion

        public static void ChangeInput(string groupName, string keyName, int index, KeyCode newKey)
        {
            if (!MapLoaded) return;
            if (!TryGetInputAction(groupName, keyName, out InputAction action, true)) return;

            CreateUserKey(groupName, keyName, index);
            UserKeys[groupName][keyName][index] = newKey;

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

        #region Get KeyCode
        public static KeyCode GetKeyCode(string actionName, int index) =>
            GetKeyCode(actionName, index, MapLoaded ? Map.DefaultGroupName : string.Empty);

        public static KeyCode GetKeyCode(string actionName, int index, string groupName)
        {
            TryGetKeyCode(actionName, index, groupName, out KeyCode key, true);
            return key;
        }

        public static bool TryGetKeyCode(string actionName, int index, out KeyCode key, bool logError) =>
            TryGetKeyCode(actionName, index, MapLoaded ? Map.DefaultGroupName : string.Empty, out key, logError);

        public static bool TryGetKeyCode(string actionName, int index, string groupName, out KeyCode key, bool logError = false)
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

        public static float GetMapAxisRaw(string groupName, string axisName) =>
            GetMapAxis(groupName, axisName);

        public static float GetMapAxisRaw(string axisName) =>
            GetMapAxis(axisName);


        [Obsolete("GetAxis is outdated. If you would like to get an Input Map axis use GetMapAxis instead and if not, use CreateAxis")]
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