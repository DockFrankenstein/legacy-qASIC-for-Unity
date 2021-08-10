using qASIC.Console;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagement
{
    public static class InputManager
    {
        #region Force
        private static Dictionary<string, bool> _forcedKeys = new Dictionary<string, bool>();

        public static void ForceKey(string keyName, bool state)
        {
            if (_forcedKeys.ContainsKey(keyName))
            {
                _forcedKeys[keyName] = state;
                return;
            }
            _forcedKeys.Add(keyName, state);
        }
        #endregion

        private static InputManagerKeys _global;
        public static InputManagerKeys GlobalKeys
        {
            get 
            {
                if (_global == null) _global = new InputManagerKeys();
                return _global;
            }
            set
            {
                _global = new InputManagerKeys(value);
            }
        }

        public static void SaveKeys()
        {
            string path = $"{Application.persistentDataPath}/{GlobalKeys.SavePath}";
            foreach (var entry in GlobalKeys.Presets)
                FileManagement.ConfigController.SetSettingFromFile(path, entry.Key, entry.Value.ToString());
        }

        public static void LoadUserKeys()
        {
            string path = $"{Application.persistentDataPath}/{GlobalKeys.SavePath}";
            if (!FileManagement.FileManager.TryLoadFileWriter(path, out string content)) return;

            List<string> settings = FileManagement.ConfigController.CreateOptionList(content);

            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].StartsWith("#")) continue;
                string[] values = settings[i].Split(':');
                if (values.Length != 2) continue;
                if (GlobalKeys.Presets.ContainsKey(values[0]) && System.Enum.TryParse(values[1], out KeyCode result)) GlobalKeys.Presets[values[0]] = result;
            }
        }

        public static bool GetInputDown(string keyName)
        {
            if (GlobalKeys == null) return false;
            if (!GlobalKeys.Presets.ContainsKey(keyName)) return false;
            return Input.GetKeyDown(GlobalKeys.Presets[keyName]);
        }

        public static bool GetInput(string keyName)
        {
            if (GlobalKeys == null) return false;
            bool isForced = _forcedKeys.ContainsKey(keyName) ? _forcedKeys[keyName] : false;
            if (!GlobalKeys.Presets.ContainsKey(keyName)) return isForced;
            return Input.GetKey(GlobalKeys.Presets[keyName]) || isForced;
        }

        public static bool GetInputUp(string keyName)
        {
            if (GlobalKeys == null) return false;
            if (!GlobalKeys.Presets.ContainsKey(keyName)) return false;
            return Input.GetKeyUp(GlobalKeys.Presets[keyName]);
        }

        public static void ChangeInput(string keyName, KeyCode newKey)
        {
            if (GlobalKeys == null || !GlobalKeys.Presets.ContainsKey(keyName)) return;
            GlobalKeys.Presets[keyName] = newKey;
            SaveKeys();
            GameConsoleController.Log($"Changed input <b>{keyName}</b> to: {newKey}", "input", Console.Logic.GameConsoleLog.LogType.Game);
        }

        public static float GetAxis(string positive, string negative)
        {
            float value = 0f;
            if (GetInput(positive)) value++;
            if (GetInput(negative)) value--;
            return value;
        }
    }
}