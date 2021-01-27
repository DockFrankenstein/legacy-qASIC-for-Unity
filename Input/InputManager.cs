using qASIC.Console;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.InputManagment
{
    public static class InputManager
    {
        private static Color errorColor = new Color(1f, 0f, 0f);
        private readonly static InputManagerKeys keys = new InputManagerKeys();

        public static InputManagerKeys GetKeys() { return keys; }

        public static void CheckForKeys()
        {
            if (keys.values == null)
                LoadKeys();
        }

        public static void SaveKeys()
        {
            string path = $"{Application.persistentDataPath}/qASIC/InputKeys.txt";
            for (int i = 0; i < keys.values.Count; i++)
                FileManaging.ConfigController.SaveSetting(path, keys.values[i].keyName, keys.values[i].key.ToString(), "Keys");
        }

        public static void LoadKeys()
        {
            keys.values = new List<InputKeyValue>();

            string path = $"{FileManaging.FileManager.TrimPathEnd(Application.dataPath, 1)}/ProjectSettings/qASIC/InputKeys.asset";

            if (FileManaging.FileManager.TryLoadTxtFile(path, out string data))
            {
                if (FileManaging.ConfigController.TryGettingConfigGroup("Keys", FileManaging.ConfigController.Decode(data), out List<string> settings))
                    for (int i = 0; i < settings.Count; i++)
                        if (i != 0)
                        {
                            string[] setting = settings[i].Split(':');
                            if (System.Enum.TryParse(FileManaging.ConfigController.SortValue(setting[1]), out KeyCode key))
                                keys.Add(setting[0], key);
                        }
                LoadUserKeys();
            }
            else
                SaveKeys();
        }

        private static void LoadUserKeys()
        {
            string path = $"{Application.persistentDataPath}/InputKeys.txt";
            if (FileManaging.FileManager.TryLoadTxtFile(path, out string data))
            {
                if (FileManaging.ConfigController.TryGettingConfigGroup("Keys", FileManaging.ConfigController.Decode(data), out List<string> settings))
                {
                    for (int i = 0; i < settings.Count; i++)
                    {
                        if (!settings[i].StartsWith("#"))
                        {
                            string[] values = settings[i].Split(':');
                            values[1] = FileManaging.ConfigController.SortValue(values[1]);
                            if (keys.TryGetting(values[0], out _))
                                keys.Change(values[0], (KeyCode)System.Enum.Parse(typeof(KeyCode), values[1]));
                        }
                    }
                }
            }
        }

        public static bool GetInputDown(string keyName)
        {
            CheckForKeys();
            List<KeyCode> keyCodes = keys.GetKeyCodes(keyName);

            bool state = false;
            for (int i = 0; i < keyCodes.Count; i++)
                if (Input.GetKeyDown(keyCodes[i]))
                    state = true;

            if (!state)
                KeyNotFoundError(keyName);

            return state;
        }

        public static bool GetInput(string keyName)
        {
            CheckForKeys();
            List<KeyCode> keyCodes = keys.GetKeyCodes(keyName);

            bool state = false;
            for (int i = 0; i < keyCodes.Count; i++)
                if (Input.GetKey(keyCodes[i]))
                    state = true;

            if (!state)
                KeyNotFoundError(keyName);

            return state;
        }

        public static bool GetInputUp(string keyName)
        {
            CheckForKeys();
            List<KeyCode> keyCodes = keys.GetKeyCodes(keyName);

            bool state = false;
            for (int i = 0; i < keyCodes.Count; i++)
                if (Input.GetKeyUp(keyCodes[i]))
                    state = true;

            if (!state)
                KeyNotFoundError(keyName);

            return state;
        }

        private static void KeyNotFoundError(string keyName)
        { GameConsoleController.Log($"Key <b>{keyName}</b> does not exist!", "error"); }

        public static void ChangeInput(string keyName, KeyCode newKey, bool save = false)
        {
            if (!keys.TryChanging(keyName, newKey))
                KeyNotFoundError(keyName);

            if (save) SaveKeys();
        }

        public static float GetAxis(string positive, string negative)
        {
            float value = 0f;
            if (GetInput(positive)) value++;
            if (GetInput(negative)) value--;
            return value;
        }

        public static void AddInput(string keyName, KeyCode key, bool save = false) 
        { keys.Add(keyName, key); if(!save) SaveKeys(); }

        public static void RemoveInput(string keyName, bool save = false) 
        { keys.Remove(keyName); if (!save) SaveKeys(); }
    }

    public class InputManagerKeys
    {
        private Color errorColor = new Color(1f, 0f, 0f);
        public List<InputKeyValue> values = null;

        public void Add(string keyName, KeyCode key) => values.Add(new InputKeyValue(keyName, key));
        public void Remove(string keyName)
        {
            for (int i = 0; i < values.Count; i++)
                if (values[i].keyName == keyName)
                {
                    values.RemoveAt(i);
                    return;
                }
            GameConsoleController.Log($"Cannot remove key <b>{keyName}</b>", "error");
        }

        public bool TryChanging(string keyName, KeyCode newKey)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].keyName == keyName)
                {
                    values[i].key = newKey;
                    return true;
                }
            }
            return false;
        }

        public void Change(string keyName, KeyCode newKey)
        {
            if (!TryChanging(keyName, newKey))
                GameConsoleController.Log($"Key <b>{keyName}</b> does not exist!", "error");
        }

        public KeyCode Get(string keyName)
        {
            if (TryGetting(keyName, out KeyCode key))
                return key;
            GameConsoleController.Log($"Key <b>{keyName}</b> does not exist!", "error");
            return KeyCode.None;
        }

        public List<KeyCode> GetKeyCodes(string keyName)
        {
            List<KeyCode> keyCodes = new List<KeyCode>();
            for (int i = 0; i < values.Count; i++)
                if (values[i].keyName == keyName)
                    keyCodes.Add(values[i].key);
            return keyCodes;
        }

        public bool TryGetting(string keyName, out KeyCode key)
        {
            List<KeyCode> keyCodes = GetKeyCodes(keyName);
            key = KeyCode.None;

            bool wereKeyCodesFound = keyCodes.Count != 0;
            if (wereKeyCodesFound)
                key = keyCodes[1];
            return wereKeyCodesFound;
        }
    }

    public class InputKeyValue
    {
        public string keyName;
        public KeyCode key;

        public InputKeyValue(string _keyName, KeyCode _key)
        {
            keyName = _keyName;
            key = _key;
        }
    }
}