using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace qASIC.Displayer
{
    public class InfoDisplayer : MonoBehaviour
    {
        public string DisplayerName = "main";
        [Tooltip("Seperator between line name and it's value")]
        public string Seperator = ": ";
        [Tooltip("Decides if a line should be displayed if it isn't created by default")]
        public bool ExceptUnknown = true;

        [Space]
        public string[] defaultLines;

        [Space]
        public string StartText;
        public string EndText;
        public TextMeshProUGUI Text;

        [Space]
        public KeyCode ToggleKey = KeyCode.F3;
        public GameObject ToggleObject;
        bool isToggled = false;

        private Dictionary<string, InfoDisplayerLine> lines = new Dictionary<string, InfoDisplayerLine>();
        private static Dictionary<string, InfoDisplayer> displayers = new Dictionary<string, InfoDisplayer>();

        private void Awake()
        {
            if (!displayers.ContainsKey(DisplayerName))
            {
                displayers.Add(DisplayerName, this);
                Initialize();
                return;
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (displayers.ContainsKey(DisplayerName)) displayers.Remove(DisplayerName);
        }

        public void Initialize()
        {
            lines.Clear();
            for (int i = 0; i < defaultLines.Length; i++)
                if (!lines.ContainsKey(defaultLines[i]))
                    lines.Add(defaultLines[i], new InfoDisplayerLine());
        }

        private void LateUpdate()
        {
            if (Text == null) return;
            Text.text = StartText;
            foreach (var value in lines)
                if(!value.Value.Hide) Text.text += $"{value.Key}{Seperator}{value.Value.Value}\n";
            Text.text += EndText;
        }

        #region Logic
        private static bool GetDisplayer(string displayerName, out InfoDisplayer displayer)
        {
            displayer = null;
            if (!displayers.ContainsKey(displayerName)) return false;
            displayer = displayers[displayerName];
            return true;
        }

        private static bool LineExists(string lineName, InfoDisplayer displayer)
        {
            if (!displayer.lines.ContainsKey(lineName))
            {
                if (!displayer.ExceptUnknown) return false;
                displayer.lines.Add(lineName, new InfoDisplayerLine());
            }
            return true;
        }
        #endregion

        #region Toggle
        private void Update()
        {
            if (Input.GetKeyDown(ToggleKey))
                Toggle();
        }

        public void Toggle() => Toggle(!isToggled);
        public void Toggle(bool state)
        {
            isToggled = state;
            ToggleObject.SetActive(isToggled);
        }

        public static void Toggle(bool state, string displayerName = "main")
        {
            if (!GetDisplayer(displayerName, out InfoDisplayer display)) return;
            display.Toggle(state);
        }

        public static void Toggle(string displayerName = "main")
        {
            if (!GetDisplayer(displayerName, out InfoDisplayer display)) return;
            display.Toggle();
        }

        #endregion

        #region Change
        public static void DisplayValue(string lineName, string value, bool hidden, string displayerName = "main")
        {
            if (!GetDisplayer(displayerName, out InfoDisplayer display)) return;
            if (!LineExists(lineName, display)) return;
            display.lines[lineName].Value = value;
            display.lines[lineName].Hide = hidden;
        }

        public static void DisplayValue(string lineName, string value, string displayerName = "main")
        {
            if (!GetDisplayer(displayerName, out InfoDisplayer display)) return;
            if (!LineExists(lineName, display)) return;
            display.lines[lineName].Value = value;
        }

        public static void HideLine(string lineName, bool hide, string displayerName = "main")
        {
            if (!GetDisplayer(displayerName, out InfoDisplayer display)) return;
            if (!LineExists(lineName, display)) return;
            display.lines[lineName].Hide = hide;
        }
        #endregion
    }
}