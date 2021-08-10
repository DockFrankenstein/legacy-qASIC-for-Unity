using UnityEngine;

namespace qASIC.Displayer.Displayers
{
    public class BasicDisplayer : MonoBehaviour
    {
        public string displayerName = "main";

        [Header("Settings")]
        public DisplayerValueAssigner framerate = new DisplayerValueAssigner("fps");
        public DisplayerValueAssigner resolution = new DisplayerValueAssigner("resolution");
        public DisplayerValueAssigner fullscreen = new DisplayerValueAssigner("fullscreen");
        public DisplayerValueAssigner GPU = new DisplayerValueAssigner("gpu");
        public DisplayerValueAssigner CPU = new DisplayerValueAssigner("cpu");
        public DisplayerValueAssigner CPUThreads = new DisplayerValueAssigner("cpu threads");
        public DisplayerValueAssigner memory = new DisplayerValueAssigner("memory");
        public DisplayerValueAssigner OS = new DisplayerValueAssigner("os");


        float time;
        int framecount;

        private void Start()
        {
            framerate.DisplayValue($"{1f / Time.deltaTime} {Time.deltaTime * 1000f}ms", displayerName);
            resolution.DisplayValue($"{Screen.currentResolution.width}x{Screen.currentResolution.height}", displayerName);
            fullscreen.DisplayValue(Screen.fullScreenMode.ToString(), displayerName);
            GPU.DisplayValue(SystemInfo.graphicsDeviceName, displayerName);
            CPU.DisplayValue(SystemInfo.processorType, displayerName);
            CPUThreads.DisplayValue(SystemInfo.processorCount.ToString(), displayerName);
            memory.DisplayValue($"{SystemInfo.systemMemorySize}MB", displayerName);
            OS.DisplayValue(SystemInfo.operatingSystem.ToString(), displayerName);
        }

        private void Update()
        {
            DisplayFramerate();
            resolution.DisplayValue($"{Screen.currentResolution.width}x{Screen.currentResolution.height}", displayerName);
            fullscreen.DisplayValue(Screen.fullScreenMode.ToString(), displayerName);
        }

        private void Reset()
        {
            InfoDisplayer infoDisplayer = GetComponent<InfoDisplayer>();
            if (infoDisplayer != null) displayerName = infoDisplayer.displayerName;
        }

        private void DisplayFramerate()
        {
            time += Time.deltaTime;
            framecount++;
            if (time >= 1)
            {
                framerate.DisplayValue($"{framecount} {time / framecount * 1000f}ms", displayerName);
                framecount = 0;
            }
            time %= 1;
        }
    }
}