namespace qASIC.Displayer
{
    [System.Serializable]
    public class DisplayerLine
    {
        public string tag;
        public string text;
        public string value;
        public bool show = true;

        public DisplayerLine()
        {
            tag = string.Empty;
            text = string.Empty;
            value = string.Empty;
            show = true;
        }

        public DisplayerLine(string tag, string text, string value, bool show)
        {
            this.tag = tag;
            this.text = text;
            this.value = value;
            this.show = show;
        }

        public DisplayerLine(string tag, string text, bool show)
        {
            this.tag = tag;
            this.text = text;
            this.show = show;
        }

        public DisplayerLine(string tag, string text)
        {
            this.tag = tag;
            this.text = text;
        }
    }
}