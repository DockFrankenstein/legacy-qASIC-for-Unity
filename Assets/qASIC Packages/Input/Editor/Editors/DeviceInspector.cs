using qASIC.EditorTools;
using UnityEditor;
using UnityEngine;
using qASIC.Input.Devices;
using System.Linq;

namespace qASIC.Input.DebugTools
{
    public class DeviceInspector : EditorWindow, IHasCustomMenu
    {
        int _selectedIndex = -1;

        Vector2 _deviceScrollPosition;
        Vector2 _propertyScrollPosition;

        bool _rightClickPressed;

        [MenuItem("Window/qASIC/Input/Device Inspector")]
        public static DeviceInspector OpenWindow()
        {
            DeviceInspector window = GetEditorWindow();
            window.minSize = new Vector2(250f, 400f);
            window.Show();
            return window;
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem("Reload Device Manager", false, () => DeviceManager.Reload());
        }

        private void Update()
        {
            Repaint();
        }

        public static DeviceInspector GetEditorWindow() =>
            (DeviceInspector)GetWindow(typeof(DeviceInspector), false, "Device Inspector");

        private void OnGUI()
        {
            var devices = DeviceManager.Devices;

            Event e = Event.current;

            if (e.rawType == EventType.MouseDown && e.button == 1)
            {
                _rightClickPressed = true;
            }

            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();
                //Preferences.AutoRefresh = GUILayout.Toggle(Preferences.AutoRefresh, "Auto Refresh", EditorStyles.toolbarButton);

                //using (new EditorGUI.DisabledScope(Preferences.AutoRefresh))
                //{
                //    if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                //        RefreshDevices();
                //}

                EditorGUILayout.Space();
            }

            //List
            using (var scroll = new GUILayout.ScrollViewScope(_deviceScrollPosition))
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    if (GUILayout.Toggle(_selectedIndex == i, $"{devices[i].DeviceName} ({devices[i].GetType().Name})", Styles.ListItemStyle))
                    {
                        _selectedIndex = i;
                    }

                    Rect r = GUILayoutUtility.GetLastRect();
                    if (_rightClickPressed && r.Contains(e.mousePosition))
                        OpenItemMenu(i);
                }

                _deviceScrollPosition = scroll.scrollPosition;
            }

            //Data
            using (new GUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Height(200f)))
            {
                using (var scroll = new GUILayout.ScrollViewScope(_propertyScrollPosition))
                {
                    EditorGUILayout.LabelField("Properties", EditorStyles.whiteLargeLabel);
                    EditorGUI.indentLevel++;

                    if (devices.IndexInRange(_selectedIndex))
                    {
                        var device = devices[_selectedIndex];

                        EditorGUILayout.LabelField("Values");
                        EditorGUI.indentLevel++;
                        var values = device.Values
                            .Where(x => x.Value != 0f);
                        foreach (var item in values)
                            EditorGUILayout.LabelField($"{item.Key}: {item.Value}");

                        if (values.Count() == 0)
                            EditorGUILayout.LabelField("EMPTY");

                        EditorGUI.indentLevel--;

                        var properties = device.GetProperties();
                        foreach (var item in properties)
                            EditorGUILayout.LabelField($"{item.Key}: {item.Value}");
                    }

                    EditorGUI.indentLevel--;
                    _propertyScrollPosition = scroll.scrollPosition;
                }
            }

            if (e.type == EventType.Repaint)
                _rightClickPressed = false;
        }

        void OpenItemMenu(int i)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem("Force disconnect", false, () => DeviceManager.DeregisterDevice(DeviceManager.Devices[i]));

            menu.ShowAsContext();
        }

        private static class Styles
        {
            public static GUIStyle ListItemStyle => new GUIStyle(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft, };
        }
    }
}