#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qASIC.FileManaging;

namespace qASIC.InputManagment.Editor
{
    public class InputWindow : EditorWindow
    {
        [MenuItem("Window/qASIC/Input window")]
        public static void ShowWindow()
        {
            InputWindow window = (InputWindow)GetWindow(typeof(InputWindow));
            //InputManager.CheckForKeys();
            window.Show();
        }

        private void OnEnable()
        {
            //InputManager.CheckForKeys();
        }

        private List<string> inputs = new List<string>();

        private string GetPath()
        {
            return FileManager.TrimPathEnd(Application.dataPath, 1) + "/ProjectSettings/qASIC/InputKeys.asset";
        }

        private void GetKeys()
        {
            string path = GetPath();
            if (FileManager.TryLoadTxtFile(path, out string config))
            {
                List<List<string>> data = ConfigController.Decode(config);
                if (!ConfigController.TryGettingConfigGroup("Keys", data, out inputs))
                    ConfigController.AddGroup(path, "Keys");
            }
            if (inputs.Count != 0 && inputs[0].StartsWith("#")) inputs.RemoveAt(0);
        }

        private void Save()
        {
            string path = GetPath();
            if (FileManager.TryLoadTxtFile(path, out string data))
            {
                ConfigController.DeleteGroup("Keys", path);
                List<List<string>> config;
                for (int i = 0; i < inputs.Count; i++)
                {
                    config = ConfigController.Decode(data);
                    string[] values = inputs[i].Split(':');

                    /*if (ConfigController.OptionExistsInGroup(config, values[0], "Keys"))
                    { inputs[i] = RenameKey(values[0], config) + ": " + values[1]; Debug.Log(inputs[i] + " has been renamed"); }*/

                    if (values[0] == "" || values[1] == "") { inputs.RemoveAt(i); i--; }
                    else
                    {
                        if (values.Length >= 2)
                            ConfigController.SaveSetting(path, values[0], ConfigController.SortValue(values[1]), "Keys");
                        else
                            ConfigController.SaveSetting(path, values[0], "", "Keys");
                    }
                }
            }
            GetKeys();
        }

        /*private string RenameKey(string keyName, List<List<string>> config)
        {
            Debug.Log(keyName);
            if (keyName.Contains("[") && keyName.EndsWith("]"))
            {
                char[] chars = keyName.ToCharArray();
                int trimValue = 0;
                for (int i = 0; i < chars.Length; i++)
                    if (chars[i] == '[') trimValue = i;

                keyName = keyName.Remove(trimValue, chars.Length - trimValue);
            }
            int newIndex = 0;
            for (; ConfigController.OptionExistsInGroup(config, keyName + "[" + newIndex + "]", "Keys"); newIndex++) 
            {  }
            return keyName + "[" + newIndex + "]";
        }*/

        public void OnGUI()
        {
            if (inputs.Count == 0)
                GetKeys();

            GUILayout.Label("qASIC input window");

            GUILayout.BeginVertical("HelpBox");

            for (int i = 0; i < inputs.Count; i++)
            {
                GUILayout.BeginHorizontal("GroupBox");
                string keyName = GUILayout.TextField(inputs[i].Split(':')[0]);
                string key = GUILayout.TextField(ConfigController.SortValue(inputs[i].Split(':')[1]));
                inputs[i] = keyName + ": " + key;
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add"))
            {
                inputs.Add("newKey: Space");
            }

            if (GUILayout.Button("Manual save")) Save();
            if (GUILayout.Button("Manual load")) GetKeys();

            GUILayout.EndVertical();
        }
    }
}
#endif