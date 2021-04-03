using UnityEngine;

namespace qASIC.Displayer.Displayers
{
    public class VersionDisplayer : MonoBehaviour
    {
        public string DisplayerName = "main";

        [Header("Settings")]
        public DisplayerLineSettings Project = new DisplayerLineSettings("Version");
        public DisplayerLineSettings Unity = new DisplayerLineSettings("Unity version");
        public DisplayerLineSettings qASIC = new DisplayerLineSettings("qASIC version");

        private void Start()
        {
            Project.DisplayValue(Application.version, DisplayerName);
            Unity.DisplayValue(Application.unityVersion, DisplayerName);
            qASIC.DisplayValue(Backend.Info.Version, DisplayerName);
        }
    }
}