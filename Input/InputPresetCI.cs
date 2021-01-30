#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace qASIC.InputManagment.Tools
{
    [CustomEditor(typeof(InputPreset))]
    public class InputPresetCI : Editor
    {
        public InputPreset preset;

        int currentEdit = -1;
        string newKeyName = "";

        private void OnEnable() => preset = (InputPreset)target;

        public override void OnInspectorGUI()
        {
            preset.preset.savePath = EditorGUILayout.TextField("Save path", preset.preset.savePath);
            DisplayList();

            GUILayout.BeginVertical("Toolbar");
            GUILayout.Label("");
            GUILayout.EndVertical();
        }

        private void DisplayList()
        {
            string[] keyCodes = Enum.GetNames(typeof(KeyCode));
            //index = EditorGUILayout.Popup(index, keyCodes);

            Dictionary<string, KeyCode> keys = preset.preset.presets;
            List<string> keyList = new List<string>(keys.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                KeyCode key = preset.preset.presets[keyList[i]];
                GUILayout.BeginHorizontal();
                if (currentEdit == i)
                {
                    newKeyName = EditorGUILayout.TextField(newKeyName);
                    if (Event.current.keyCode == KeyCode.Return)
                    {
                        currentEdit = -1;
                        key = preset.preset.presets[keyList[i]];
                        preset.preset.presets.Remove(keyList[i]);
                        preset.preset.presets.Add(CreateNewName(newKeyName), key);
                    }
                }
                else if (GUILayout.Button(keyList[i].ToString()))
                {
                    currentEdit = i;
                    newKeyName = keyList[i];
                }
                key = (KeyCode)EditorGUILayout.EnumPopup(key);
                if (preset.preset.presets.ContainsKey(keyList[i]))
                    preset.preset.presets[keyList[i]] = key;
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add")) preset.preset.presets.Add(CreateNewName("newKey"), KeyCode.A);
            if (GUILayout.Button("Save")) { }
        }

        private string CreateNewName(string baseName)
        {
            string newName = baseName;
            int newIndex = 0;
            if (!preset.preset.presets.ContainsKey(newName)) { return newName; }
            while (preset.preset.presets.ContainsKey(newName))
            {
                newName = $"{baseName}{newIndex}";
                if (newIndex >= 1024)
                {
                    Debug.LogError("Index is out of range");
                    return "";
                }
                newIndex++;
            }
            return newName;
        }
    }
}
#endif