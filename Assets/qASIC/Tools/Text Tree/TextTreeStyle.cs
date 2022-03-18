using UnityEngine;

namespace qASIC.Tools
{
    [System.Serializable]
    public class TextTreeStyle
    {
        public enum Preset { custom, fancy, basic }

        [SerializeField] private string middle;
        [SerializeField] private string last;
        [SerializeField] private string space;
        [SerializeField] private string vertical;
        [SerializeField] private Preset preset = Preset.custom;

        public string Middle { get => middle; set => middle = value; }
        public string Last { get => last; set => last = value; }
        public string Space { get => space; set => space = value; }
        public string Vertical { get => vertical; set => vertical = value; }

        public TextTreeStyle() =>
            SetPreset(Preset.fancy);

        public TextTreeStyle(Preset preset) =>
            SetPreset(preset);

        public TextTreeStyle(string middle, string last, string space, string vertical)
        {
            this.middle = middle;
            this.last = last;
            this.space = space;
            this.vertical = vertical;
        }

        public void SetPreset(Preset preset)
        {
            this.preset = preset;

            if (preset != Preset.custom)
                ModifyPreset(preset, out middle, out last, out space, out vertical);
        }

        public static void ModifyPreset(Preset preset, out string middle, out string last, out string space, out string vertical)
        {
            middle = string.Empty;
            last = string.Empty;
            space = string.Empty;
            vertical = string.Empty;

            switch (preset)
            {
                case Preset.fancy:
                    middle = " ├─";
                    last = " └─";
                    space = "   ";
                    vertical = " │ ";
                    break;
                case Preset.basic:
                    middle = "+-";
                    last = "+-";
                    space = "   ";
                    vertical = " │ ";
                    break;
            }
        }
    }
}