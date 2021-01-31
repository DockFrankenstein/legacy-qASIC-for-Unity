#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Reflection;

namespace qASIC.InputManagment.Tools
{
    [CustomEditor(typeof(InputPreset))]
    public class InputPresetCI : Editor
    {
        public InputPreset preset;

        int currentEdit = -1;
        string newKeyName = "";

        Dictionary<string, KeyCode> keys;

        private void OnEnable() 
        { 
            preset = (InputPreset)target;
            keys = preset.preset.presets;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            preset.preset.savePath = EditorGUILayout.TextField("Save path", preset.preset.savePath);
            DisplayList();
            EditorUtility.SetDirty(preset);
        }

        private void DisplayList()
        {
            GUIStyle textStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft
            };

            GUIStyle fieldStyle = new GUIStyle("Textfield")
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };

            GUILayout.BeginVertical();
            Dictionary<string, KeyCode> keys = preset.preset.presets;
            List<string> keyList = new List<string>(keys.Keys);

            for (int i = 0; i < keyList.Count; i++)
            {
                KeyCode key = keys[keyList[i]];
                GUILayout.BeginHorizontal("TextArea", GUILayout.Height(22));
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                if (currentEdit == i)
                {
                    newKeyName = EditorGUILayout.TextField(newKeyName, fieldStyle, GUILayout.Height(20));
                    Event e = Event.current;
                    if (e.keyCode == KeyCode.Return || (e.button == 0 && e.isMouse)) ResetEdit(keyList);
                }
                else if (GUILayout.Button(keyList[i].ToString(), textStyle, GUILayout.Height(20)))
                {
                    if(currentEdit != -1) ResetEdit(keyList);
                    currentEdit = i;
                    newKeyName = keyList[i];
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                key = (KeyCode)EditorGUILayout.EnumPopup(key, GUILayout.Width(75), GUILayout.Height(20));
                if (keys.ContainsKey(keyList[i])) keys[keyList[i]] = key;
                if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) keys.Remove(keyList[i]);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            if (GUILayout.Button("Add")) keys.Add(CreateNewName("newKey"), KeyCode.A);
            preset.preset.presets = keys;
        }

        private void ResetEdit(List<string> keyList)
        {
            KeyCode key = keys[keyList[currentEdit]];
            keys.Remove(keyList[currentEdit]);
            keys.Add(CreateNewName(newKeyName), key);
            currentEdit = -1;
            newKeyName = "";
        }

        private string CreateNewName(string baseName)
        {
            string newName = baseName;
            int newIndex = 0;
            if (!preset.preset.presets.ContainsKey(newName)) { return newName; }
            while (preset.preset.presets.ContainsKey(newName))
            {
                newName = $"{baseName}{newIndex}";
                newIndex++;
                if (newIndex >= 4096)
                {
                    Debug.LogError("Index is out of range");
                    return "";
                }
            }
            return newName;
        }
    }
}
#endif