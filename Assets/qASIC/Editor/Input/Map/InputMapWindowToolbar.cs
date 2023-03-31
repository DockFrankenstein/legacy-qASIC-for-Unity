#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using qASIC.EditorTools;
using UnityEditor.IMGUI.Controls;

using UnityObject = UnityEngine.Object;

using static UnityEngine.GUILayout;

namespace qASIC.Input.Map.Internal
{
    public class InputMapWindowToolbar
    {
        public InputMap map;
        public InputMapWindow window;
        public InputMapWindowInspector inspector;
        public InputMapWindowGroupBar groupBar;
        public InputMapWindowContentTree contentTree;

        #region GUI
        public void OnGUI()
        {
            BeginHorizontal(EditorStyles.toolbar);

            DrawMenus();
            EditorGUILayout.Space();
            Label(map ? map.name : "Not loaded");

            FlexibleSpace();

            if (contentTree != null)
                contentTree.searchString = EditorGUILayout.TextField(contentTree.searchString ?? string.Empty, EditorStyles.toolbarSearchField);

            if (Button("Show in folder", EditorStyles.toolbarButton))
                ShowInFolder();

            AutoSaveButton();

            if (Button("Save", EditorStyles.toolbarButton))
                window.Save();

            EditorGUILayout.Space();
            EndHorizontal();
        }

        void AutoSaveButton()
        {
            bool isAutoSaving = window.IsDirty && !InputMapWindow.CanAutoSave() && InputMapWindow.Prefs_AutoSave;
            string autosaveText = isAutoSaving ? $" ({Mathf.Round((float)InputMapWindow.TimeToAutoSave + 0.5f)}s)" : string.Empty;

            if (Toggle(map && InputMapWindow.Prefs_AutoSave, $"Auto save{autosaveText}", EditorStyles.toolbarButton) != InputMapWindow.Prefs_AutoSave)
                InputMapWindow.Prefs_AutoSave = !InputMapWindow.Prefs_AutoSave;

            if (isAutoSaving)
                window.Repaint();
        }
        #endregion

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

                menu.AddToggableItem("Save", false, window.Save, map);
                menu.AddToggableItem("Auto save", InputMapWindow.Prefs_AutoSave, () => { InputMapWindow.Prefs_AutoSave = !InputMapWindow.Prefs_AutoSave; }, map);
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
                menu.AddItem("Window settings", false, () => window.SetInspector(new Inspectors.InputMapWindowSettingsInspector()));
                menu.AddSeparator("");
                menu.AddToggableItem("Set map as default", false, window.SetAsDefaultProjectMapDialogue, ProjectSettings.InputProjectSettings.Instance.map != map);
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
                    menu.AddItem("Reload map", false, () => window.Map?.Initialize());
                    menu.AddSeparator("");
                    menu.AddItem("Load current map", false, window.LoadMap);
                    menu.AddItem("Print current map", false, () => Debug.Log(InputMapWindow.GetMapPath()));
                    menu.AddSeparator("");

                    menu.AddItem("Open unmodified", false, () =>
                        System.Diagnostics.Process.Start(InputMapWindow.GetUnmodifiedMapLocation()));

                    menu.AddItem("Open unmodified in explorer", false, () => 
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{InputMapWindow.GetUnmodifiedMapLocation().Replace('/', '\\')}\""));

                    menu.AddSeparator("");
                    menu.AddItem("Window Inspector/Reset inspector", false, () => window.SelectInInspector(null));
                    menu.AddItem("Window Inspector/Rebuild inspectors", false, () => inspector.RebuildInspectors());
                    menu.AddSeparator("");
                    menu.AddItem("Map/Rebuild Item Cache", false, () => map.RebuildItemCache());
                    menu.AddSeparator("");
                    menu.AddToggableItem($"{(InputMapWindow.Prefs_AutoSave ? "*" : "")}Set dirty", false, window.SetMapDirty, map);
                    menu.AddToggableItem("Close map", false, window.CloseMap, map);
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
#endif