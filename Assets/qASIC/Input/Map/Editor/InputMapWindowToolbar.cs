﻿using UnityEngine;
using UnityEditor;
using System;
using qASIC.EditorTools;

using UnityObject = UnityEngine.Object;

using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapWindowToolbar
    {
        public InputMap map;

        public void OnGUI()
        {
            BeginHorizontal(EditorStyles.toolbar);

            DrawMenus();
            EditorGUILayout.Space();
            Label(map ? map.name : "Not loaded");

            FlexibleSpace();

            Label((InputMapWindow.IsDirty && !InputMapWindow.CanAutoSave()) ? $"Auto save delayed ({Mathf.Round(InputMapWindow.AutoSaveTimeLimit)}s)" : "");

            if (Button("Show in folder", EditorStyles.toolbarButton))
                ShowInFolder();

            if (Toggle(map && InputMapWindow.AutoSave, "Auto save", EditorStyles.toolbarButton) != InputMapWindow.AutoSave)
                InputMapWindow.AutoSave = !InputMapWindow.AutoSave;

            if (Button("Save", EditorStyles.toolbarButton))
                InputMapWindow.Save();

            EditorGUILayout.Space();
            EndHorizontal();
        }

        #region Menus
        Rect fileMenuRect;
        Rect settingsMenuRect;
        Rect helpMenuRect;
        Rect debugMenuRect;

        void DrawMenus()
        {
            DisplayMenu("File", ref fileMenuRect, (GenericMenu menu) =>
            {
                InputMapWindow window = InputMapWindow.GetEditorWindow();

                menu.AddToggableItem("Save", false, InputMapWindow.Save, map);
                menu.AddToggableItem("Auto save", InputMapWindow.AutoSave, () => { InputMapWindow.AutoSave = !InputMapWindow.AutoSave; }, map);
                menu.AddToggableItem("Show in folder", false, ShowInFolder, map);
                menu.AddSeparator("");
                menu.AddItem("Open", false, OpenAsset);
                menu.AddToggableItem("Discard changes", false, window.DiscardMapChanges, map);
                menu.AddSeparator("");
                menu.AddItem("Close", false, InputMapWindow.GetEditorWindow().Close);
            });

            DisplayMenu("Settings", ref settingsMenuRect, (GenericMenu menu) =>
            {
                InputMapWindow window = InputMapWindow.GetEditorWindow();

                menu.AddItem("Open project settings", false, () => SettingsService.OpenProjectSettings("Project/qASIC/Input"));

                if (InputMapWindow.DebugMode)
                {
                    menu.AddSeparator("");
                    menu.AddItem("Window settings (experimental)", false, () => window.SelectInInspector("settings"));
                }
            });

            DisplayMenu("Help", ref helpMenuRect, (GenericMenu menu) =>
            {
                menu.AddItem("Documentation", false, () => Application.OpenURL("https://docs.qasictools.com/docs/input/map"));
                menu.AddDisabledItem("Guides (comming not so soon)", false/*, () => Application.OpenURL("https://docs.qasictools.com")*/);
                menu.AddSeparator("");
                menu.AddItem("Support", false, () => Application.OpenURL("https://qasictools.com/support#support"));
            });

            if (InputMapWindow.DebugMode)
            {
                DisplayMenu("Debug", ref debugMenuRect, (GenericMenu menu) =>
                {
                    InputMapWindow window = InputMapWindow.GetEditorWindow();

                    menu.AddItem("Update name", false, window.SetWindowTitle);
                    menu.AddItem("Reset editor", false, window.ResetEditor);
                    menu.AddItem("Reload trees", false, window.ReloadTrees);
                    menu.AddItem("Reset preferences", false, InputMapWindow.ResetPreferences);
                    menu.AddSeparator("");
                    menu.AddToggableItem($"{(InputMapWindow.AutoSave ? "*" : "")}Set dirty", false, InputMapWindow.SetMapDirty, map);
                    menu.AddToggableItem("Close map", false, InputMapWindow.CloseMap, map);
                });
            }
        }

        void DisplayMenu(string buttonText, ref Rect rect, Action<GenericMenu> menuFunction)
        {
            bool openMenu = Button(buttonText, EditorStyles.toolbarDropDown);

            //Calculating rect
            //This is a really janky solution, but sometimes GetLastRect returns 0 0
            //so in order to create the generic menu in the correct position, the rect
            //needs to be saved if the x position is not equal 0
            Rect r = GUILayoutUtility.GetLastRect();

            if (rect == null)
                rect = r;

            if (r.x != 0)
                rect = r;


            if (!openMenu) return;
            GenericMenu menu = new GenericMenu();
            menuFunction?.Invoke(menu);
            menu.DropDown(rect);
        }
        #endregion

        #region Save&Load
        void ShowInFolder()
        {
            if (map == null) return;
            Selection.SetActiveObjectWithContext(map, null);
            EditorGUIUtility.PingObject(map);
            EditorUtility.FocusProjectWindow();
        }

        void OpenAsset()
        {
            string path = EditorUtility.OpenFilePanel("Open input map", "", "asset");

            //The window was closed
            if (string.IsNullOrEmpty(path))
                return;
            
            //User tried opening a file outside of the project
            if (!path.StartsWith(Application.dataPath))
            {
                OpenError("Cannot open Input Map that is outside of the project!");
                return;
            }

            path = $"Assets{path.Remove(0, Application.dataPath.Length)}";

            UnityObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject));

            //Wrong file type
            if (!(obj is InputMap))
            {
                OpenError("Please select an Input Map!");
                return;
            }

            InputMapWindow.OpenMap(obj as InputMap);
        }

        void OpenError(string error = "Something went wrong")
        {
            EditorUtility.DisplayDialog("Couldn't load map!", error, "Ok");
        }
        #endregion
    }
}