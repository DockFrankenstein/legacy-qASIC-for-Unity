using UnityEngine;
using UnityEditor;
using System;
using qASIC.EditorTools;

using UnityObject = UnityEngine.Object;

using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Internal
{
    public class InputMapToolbar
    {
        public InputMap map;

        public void OnGUI()
        {
            BeginHorizontal(EditorStyles.toolbar);

            DrawMenus();
            EditorGUILayout.Space();
            Label(map ? map.name : "Not loaded");

            FlexibleSpace();

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
        Rect debugMenuRect;
        Rect helpMenuRect;

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
                menu.AddSeparator("");
                menu.AddItem("Close", false, InputMapWindow.GetEditorWindow().Close);
            });

            DisplayMenu("Help", ref helpMenuRect, (GenericMenu menu) =>
            {
                menu.AddItem("Documentation", false, () => Application.OpenURL("https://docs.qasictools.com/input/getting-started"));
            });

            DisplayMenu("Debug", ref debugMenuRect, (GenericMenu menu) =>
            {
                InputMapWindow window = InputMapWindow.GetEditorWindow();

                menu.AddToggableItem("Close map", false, InputMapWindow.CloseMap, map);
                menu.AddToggableItem("Set dirty", false, InputMapWindow.SetMapDirty, map);
                menu.AddToggableItem("Update name", false, window.SetWindowTitle, map);
            });
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