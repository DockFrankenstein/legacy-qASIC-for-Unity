using UnityEngine;
using UnityEngine.Serialization;
using qASIC;

namespace qASIC.Displayer.Displayers
{
    [AddComponentMenu("qASIC/Displayer/Verion Displayer")]
    public class VersionDisplayer : MonoBehaviour
    {
        public string displayerName = "main";

        [Header("Settings")]
        public DisplayerValueAssigner project = new DisplayerValueAssigner("version");
        public DisplayerValueAssigner unity = new DisplayerValueAssigner("unity version");
        [InspectorName("qASIC")] [FormerlySerializedAs("qASIC")] public DisplayerValueAssigner qasic = new DisplayerValueAssigner("qasic version");

        private void Start()
        {
            project.DisplayValue(Application.version, displayerName);
            unity.DisplayValue(Application.unityVersion, displayerName);
            qasic.DisplayValue(qASIC.qInfo.Version, displayerName);
        }

        private void Reset()
        {
            InfoDisplayer infoDisplayer = GetComponent<InfoDisplayer>();
            if (infoDisplayer != null) displayerName = infoDisplayer.displayerName;
        }
    }
}