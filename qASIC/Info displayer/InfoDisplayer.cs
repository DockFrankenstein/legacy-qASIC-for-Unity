using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace qASIC.Displayer
{
    public class InfoDisplayer : MonoBehaviour
    {
        public string DisplayerName = "main";
        [Tooltip("Separator between line name and it's value")]
        public string Separator = ": ";
        [Tooltip("Decides if a line should be displayed if it isn't created by default")]
        public bool ExceptUnknown = true;

        [Space]
        [Tooltip("If the text of a line is empty, the separator will be removed from it")]
        public bool RemoveSeparatorText = true;
        [Tooltip("If the value of a line is empty, the separator will be removed from it")]
        public bool RemoveSeparatorValue = false;

        [Space]
        public DisplayerLine[] DefaultLines = new DisplayerLine[]
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
        public string StartText;
        public string EndText;
        public TextMeshProUGUI Text;

        private Dictionary<string, DisplayerLine> lines = new Dictionary<string, DisplayerLine>();
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
            for (int i = 0; i < DefaultLines.Length; i++)
                if (!lines.ContainsKey(DefaultLines[i].tag))
                    lines.Add(DefaultLines[i].tag, DefaultLines[i]);
        }

        private void LateUpdate()
        {
            if (Text == null) return;
            Text.text = StartText;
            foreach (var value in lines)
            {
                if (!value.Value.show) continue;
                string separator = Separator;
                if ((RemoveSeparatorText && string.IsNullOrWhiteSpace(value.Value.text)) || (RemoveSeparatorValue && string.IsNullOrWhiteSpace(value.Value.value))) separator = string.Empty;
                Text.text += $"{value.Value.text}{separator}{value.Value.value}\n";
            }
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

        private static bool LineExists(string tag, InfoDisplayer displayer)
        {
            if (!displayer.lines.ContainsKey(tag))
            {
                if (!displayer.ExceptUnknown) return false;
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