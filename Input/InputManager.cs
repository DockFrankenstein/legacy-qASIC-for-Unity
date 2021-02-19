using qASIC.Console;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    public static class InputManager
    {
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
                _global = value;
            }
        }

        public static void SaveKeys() => SaveKeys(GlobalKeys);
        public static void SaveKeys(InputManagerKeys keys)
        {
            string path = $"{Application.persistentDataPath}/{keys.SavePath}";
            foreach (var entry in keys.Presets)
                FileManaging.ConfigController.SetSetting(path, entry.Key, entry.Value.ToString());
        }

        public static void LoadUserKeys() => GlobalKeys = LoadUserKeys(GlobalKeys);
        public static InputManagerKeys LoadUserKeys(InputManagerKeys keys)
        {
            string path = $"{Application.persistentDataPath}/{keys.SavePath}";
            if (!FileManaging.FileManager.TryLoadFileWriter(path, out string content)) return keys;
            List<string> settings = FileManaging.ConfigController.CreateOptionList(content);

            for (int i = 0; i < settings.Count; i++)
            {
                if (!settings[i].StartsWith("#")) continue;
                string[] values = settings[i].Split(':');
                if (values.Length != 2) continue;
                if (keys.Presets.ContainsKey(values[0]) && System.Enum.TryParse(values[1], out KeyCode result)) keys.Presets[values[1]] = result;
            }
            return keys;
        }

        public static bool GetInputDown(string keyName) => GetInputDown(GlobalKeys, keyName);
        public static bool GetInputDown(InputManagerKeys keys, string keyName)
        {
            if (keys == null) return false;
            if (!keys.Presets.ContainsKey(keyName)) return false;
            return Input.GetKeyDown(keys.Presets[keyName]);
        }

        public static bool GetInput(string keyName) => GetInput(GlobalKeys, keyName);
        public static bool GetInput(InputManagerKeys keys, string keyName)
        {
            if (keys == null) return false;
            if (!keys.Presets.ContainsKey(keyName)) return false;
            return Input.GetKey(keys.Presets[keyName]);
        }

        public static bool GetInputUp(string keyName) => GetInputUp(GlobalKeys, keyName);
        public static bool GetInputUp(InputManagerKeys keys, string keyName)
        {
            if (keys == null) return false;
            if (!keys.Presets.ContainsKey(keyName)) return false;
            return Input.GetKeyUp(keys.Presets[keyName]);
        }

        public static void ChangeInput(string keyName, KeyCode newKey) => GlobalKeys = ChangeInput(GlobalKeys, keyName, newKey);
        public static InputManagerKeys ChangeInput(InputManagerKeys keys, string keyName, KeyCode newKey)
        {
            if (keys == null || !keys.Presets.ContainsKey(keyName)) return keys;
            keys.Presets[keyName] = newKey;
            SaveKeys(keys);
            GameConsoleController.Log($"Changed input <b>{keyName}</b> to: {newKey}", "input", Console.Logic.GameConsoleLog.LogType.game);
            return keys;
        }

        public static float GetAxis(string positive, string negative) => GetAxis(GlobalKeys, positive, negative);
        public static float GetAxis(InputManagerKeys keys, string positive, string negative)
        {
            float value = 0f;
            if (GetInput(keys, positive)) value++;
            if (GetInput(keys, negative)) value--;
            return value;
        }
    }
}