using UnityEngine;

namespace qASIC.Displayer.Displayers
{
    public class VersionDisplayer : MonoBehaviour
    {
        public string DisplayerName = "main";

        [Header("Settings")]
        public DisplayerValueAssigner Project = new DisplayerValueAssigner("version");
        public DisplayerValueAssigner Unity = new DisplayerValueAssigner("unity version");
        public DisplayerValueAssigner qASIC = new DisplayerValueAssigner("qasic version");

        private void Start()
        {
            Project.DisplayValue(Application.version, DisplayerName);
            Unity.DisplayValue(Application.unityVersion, DisplayerName);
            qASIC.DisplayValue(Tools.Info.Version, DisplayerName);
        }

        private void Reset()
        {
            InfoDisplayer infoDisplayer = GetComponent<InfoDisplayer>();
            if (infoDisplayer != null) DisplayerName = infoDisplayer.DisplayerName;
        }
    }
}