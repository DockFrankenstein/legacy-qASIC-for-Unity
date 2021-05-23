using UnityEngine;

namespace qASIC.Displayer.Displayers
{
    public class VersionDisplayer : MonoBehaviour
    {
        public string DisplayerName = "main";

        [Header("Settings")]
        public DisplayerValueAssigner Project = new DisplayerValueAssigner("Version");
        public DisplayerValueAssigner Unity = new DisplayerValueAssigner("Unity version");
        public DisplayerValueAssigner qASIC = new DisplayerValueAssigner("qASIC version");

        private void Start()
        {
            Project.DisplayValue(Application.version, DisplayerName);
            Unity.DisplayValue(Application.unityVersion, DisplayerName);
            qASIC.DisplayValue(Backend.Info.Version, DisplayerName);
        }
    }
}