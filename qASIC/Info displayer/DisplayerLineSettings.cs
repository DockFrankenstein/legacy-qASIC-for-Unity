namespace qASIC.Displayer.Displayers
{
    [System.Serializable]
    public class DisplayerLineSettings
    {
        public bool Show = true;
        public string DisplayName;

        public DisplayerLineSettings() 
        {
            Show = true;
            DisplayName = string.Empty;
        }

        public DisplayerLineSettings(string displayName, bool show)
        {
            Show = show;
            DisplayName = displayName;
        }

        public DisplayerLineSettings(string displayName)
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