namespace qASIC.Displayer
{
    public class InfoDisplayerLine
    {
        public string Value;
        public bool Hide;

        public InfoDisplayerLine() { }

        public InfoDisplayerLine(string value, bool hide)
        {
            Value = value;
            Hide = hide;
        }
    }
}