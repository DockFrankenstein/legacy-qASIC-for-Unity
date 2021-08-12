using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace qASIC.Displayer
{
    public class InfoDisplayer : MonoBehaviour
    {
        public string displayerName = "main";
        [Tooltip("Separator between line name and it's value")]
        public string separator = ": ";
        [Tooltip("Decides if a line should be displayed if it isn't created by default")]
        public bool exceptUnknown = true;

        [Space]
        [Tooltip("If the text of a line is empty, the separator will be removed from it")]
        public bool removeSeparatorText = true;
        [Tooltip("If the value of a line is empty, the separator will be removed from it")]
        public bool removeSeparatorValue = false;

        [Space]
        public DisplayerLine[] defaultLines = new DisplayerLine[]
                {
                    new DisplayerLine("fps", "Framerate"),
                    new DisplayerLine("resolution", "Resolution"),
                    new DisplayerLine("fullscreen", "Fullscreen mode"),
                    new DisplayerLine("gpu", "Graphics card"),
                    new DisplayerLine("cpu", "Processor"),
                    new DisplayerLine("cpu threads", "Processor threads"),
                    new DisplayerLine("memory", "Memory"),
                    new DisplayerLine("os", "Operating system"),
                    new DisplayerLine("version", "Version"),
                    new DisplayerLine("unity version", "Unity version"),
                    new DisplayerLine("qasic version", "qASIC version"),
                };

        [Space]
        public string startText;
        public string endText;
        public TextMeshProUGUI text;

        private readonly Dictionary<string, DisplayerLine> lines = new Dictionary<string, DisplayerLine>();
        private static readonly Dictionary<string, InfoDisplayer> displayers = new Dictionary<string, InfoDisplayer>();

        private void Awake()
        {
            if (!displayers.ContainsKey(displayerName))
            {
                displayers.Add(displayerName, this);
                Initialize();
                return;
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (displayers.ContainsKey(displayerName) && !gameObject.scene.isLoaded) displayers.Remove(displayerName);
        }

        public void Initialize()
        {
            lines.Clear();
            for (int i = 0; i < defaultLines.Length; i++)
                if (!lines.ContainsKey(defaultLines[i].tag))
                    lines.Add(defaultLines[i].tag, defaultLines[i]);
        }

        private void LateUpdate()
        {
            if (text == null) return;
            text.text = startText;
            foreach (var value in lines)
            {
                if (!value.Value.show) continue;
                string separator = this.separator;
                if ((removeSeparatorText && string.IsNullOrWhiteSpace(value.Value.text)) || (removeSeparatorValue && string.IsNullOrWhiteSpace(value.Value.value))) separator = string.Empty;
                text.text += $"{value.Value.text}{separator}{value.Value.value}\n";
            }
            text.text += endText;
        }

        #region Logic
        private static bool GetDisplayer(string displayerName, out InfoDisplayer displayer)
        {
            displayer = null;
            if (!displayers.ContainsKey(displayerName)) return false;
            displayer = displayers[displayerName];
            return true;
        }

        private static bool LineExists(string tag, InfoDisplayer displayer)
        {
            if (!displayer.lines.ContainsKey(tag))
            {
                if (!displayer.exceptUnknown) return false;
                displayer.lines.Add(tag, new DisplayerLine());
            }
            return true;
        }
        #endregion

        #region Change
        public static void DisplayValue(string tag, string value, bool show, string displayerName = "main")
        {
            if (!GetDisplayer(displayerName, out InfoDisplayer display)) return;
            if (!LineExists(tag, display)) return;
            display.lines[tag].value = value;
            display.lines[tag].show = show;
        }

        public static void DisplayValue(string tag, string value, string displayerName = "main")
        {
            if (!GetDisplayer(displayerName, out InfoDisplayer display)) return;
            if (!LineExists(tag, display)) return;
            display.lines[tag].value = value;
        }

        public static void ToggleValue(string tag, bool show, string displayerName = "main")
        {
            if (!GetDisplayer(displayerName, out InfoDisplayer display)) return;
            if (!LineExists(tag, display)) return;
            display.lines[tag].show = show;
        }
        #endregion
    }
}