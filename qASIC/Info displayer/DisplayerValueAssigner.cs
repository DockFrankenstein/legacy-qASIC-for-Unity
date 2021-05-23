namespace qASIC.Displayer.Displayers
{
    [System.Serializable]
    public class DisplayerValueAssigner
    {
        public bool Show = true;
        public string DisplayName;

        public DisplayerValueAssigner() 
        {
            Show = true;
            DisplayName = string.Empty;
        }

        public DisplayerValueAssigner(string displayName, bool show)
        {
            Show = show;
            DisplayName = displayName;
        }

        public DisplayerValueAssigner(string displayName)
        {
            Show = true;
            DisplayName = displayName;
        }

        public void DisplayValue(string value, string displayerName = "main")
        {
            if (Show) 
                InfoDisplayer.DisplayValue(DisplayName, value, displayerName);
        }
    }
}