namespace qASIC.Displayer
{
    [System.Serializable]
    public class DisplayerValueAssigner
    {
        public bool show = true;
        public string tag;

        public DisplayerValueAssigner() 
        {
            show = true;
            tag = string.Empty;
        }

        public DisplayerValueAssigner(string displayName, bool show)
        {
            this.show = show;
            tag = displayName;
        }

        public DisplayerValueAssigner(string displayName)
        {
            show = true;
            tag = displayName;
        }

        public void DisplayValue(string value, string displayerName = "main")
        {
            if (show) 
                InfoDisplayer.DisplayValue(tag, value, displayerName);
        }

        public void ToggleValue(bool state, string displayerName = "main")
        {
            show = state;
            InfoDisplayer.ToggleValue(tag, state, displayerName);
        }
    }
}