using UnityEngine;

namespace qASIC.Displayer.Displayers
{
    [AddComponentMenu("qASIC/Displayer/Verion Displayer")]
    public class VersionDisplayer : MonoBehaviour
    {
        public string displayerName = "main";

        [Header("Settings")]
        public DisplayerValueAssigner project = new DisplayerValueAssigner("version");
        public DisplayerValueAssigner unity = new DisplayerValueAssigner("unity version");
        public DisplayerValueAssigner qASIC = new DisplayerValueAssigner("qasic version");

        private void Start()
        {
            project.DisplayValue(Application.version, displayerName);
            unity.DisplayValue(Application.unityVersion, displayerName);
            qASIC.DisplayValue(Internal.Info.Version, displayerName);
        }

        private void Reset()
        {
            InfoDisplayer infoDisplayer = GetComponent<InfoDisplayer>();
            if (infoDisplayer != null) displayerName = infoDisplayer.displayerName;
        }
    }
}