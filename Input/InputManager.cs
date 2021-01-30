using qASIC.Console;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    public static class InputManager
    {
        private static InputManagerKeys global;
        public static InputManagerKeys globalKeys
        {
            get 
            {
                if (global == null) global = new InputManagerKeys();
                return global;
            }
            set => global = value;
        }

        public static void SaveKeys() => SaveKeys(globalKeys);
        public static void SaveKeys(InputManagerKeys keys)
        {
            string path = $"{Application.persistentDataPath}/{keys.savePath}";
            List<string> keyList = new List<string>(keys.presets.Keys);
            for (int i = 0; i < keyList.Count; i++)
                //FileManaging.ConfigController.SaveSetting(path, keyList[i], keys.presets[keyList[i]].ToString(), "Keys");
                GameConsoleController.Log(keyList[i] + " " + keys.presets[keyList[i]], "default");
        }

        public static void LoadUserKeys() => globalKeys = LoadUserKeys(globalKeys);
        public static InputManagerKeys LoadUserKeys(InputManagerKeys keys)
        {
            string path = $"{Application.persistentDataPath}/{keys.savePath}";
            if (!FileManaging.FileManager.TryLoadTxtFile(path, out string data)) return keys;
            if (!FileManaging.ConfigController.TryGettingConfigGroup("Keys", FileManaging.ConfigController.Decode(data), out List<string> settings)) return keys;

            for (int i = 0; i < settings.Count; i++)
            {
                if (!settings[i].StartsWith("#"))
                {
                    string[] values = settings[i].Split(':');
                    if (values.Length == 2)
                    {
                        values[1] = FileManaging.ConfigController.SortValue(values[1]);
                        if (keys.presets.ContainsKey(values[0]))
                            keys.presets[values[1]] = (KeyCode)System.Enum.Parse(typeof(KeyCode), values[1]);
                    }
                }
            }
            return keys;
        }

        public static bool GetInputDown(string keyName) => GetInputDown(globalKeys, keyName);
        public static bool GetInputDown(InputManagerKeys keys, string keyName)
        {
            if (keys == null) return false;
            if (!keys.presets.ContainsKey(keyName)) return false;
            return Input.GetKeyDown(keys.presets[keyName]);
        }

        public static bool GetInput(string keyName) => GetInput(globalKeys, keyName);
        public static bool GetInput(InputManagerKeys keys, string keyName)
        {
            if (keys == null) return false;
            if (!keys.presets.ContainsKey(keyName)) return false;
            return Input.GetKey(keys.presets[keyName]);
        }

        public static bool GetInputUp(string keyName) => GetInputUp(globalKeys, keyName);
        public static bool GetInputUp(InputManagerKeys keys, string keyName)
        {
            if (keys == null) return false;
            if (!keys.presets.ContainsKey(keyName)) return false;
            return Input.GetKeyUp(keys.presets[keyName]);
        }

        public static void ChangeInput(string keyName, KeyCode newKey) => globalKeys = ChangeInput(globalKeys, keyName, newKey);
        public static InputManagerKeys ChangeInput(InputManagerKeys keys, string keyName, KeyCode newKey)
        {
            if (keys == null || keys.presets.ContainsKey(keyName)) return keys;
            keys.presets[keyName] = newKey;
            SaveKeys(keys);
            return keys;
        }

        public static float GetAxis(string positive, string negative) => GetAxis(globalKeys, positive, negative);
        public static float GetAxis(InputManagerKeys keys, string positive, string negative)
        {
            float value = 0f;
            if (GetInput(keys, positive)) value++;
            if (GetInput(keys, negative)) value--;
            return value;
        }
    }
}