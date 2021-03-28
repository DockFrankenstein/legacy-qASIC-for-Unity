using UnityEngine;

namespace qASIC.Displayer.Displayers
{
    public class BasicDisplayer : MonoBehaviour
    {
        public string DisplayerName = "main";

        [Header("Settings")]
        public DisplayerLineSettings Framerate = new DisplayerLineSettings("Framerate");
        public DisplayerLineSettings Resolution = new DisplayerLineSettings("Resolution");
        public DisplayerLineSettings Fullscreen = new DisplayerLineSettings("Fullscreen");
        public DisplayerLineSettings GPU = new DisplayerLineSettings("Graphics card");
        public DisplayerLineSettings CPU = new DisplayerLineSettings("Processor");
        public DisplayerLineSettings CPUThreads = new DisplayerLineSettings("Threads");
        public DisplayerLineSettings Memory = new DisplayerLineSettings("Memory");
        public DisplayerLineSettings OS = new DisplayerLineSettings("OS");


        float time;
        int framecount;

        private void Start()
        {
            Framerate.DisplayValue($"{1f / Time.deltaTime} {Time.deltaTime * 1000f}ms", DisplayerName);
            Resolution.DisplayValue($"{Screen.currentResolution.width}x{Screen.currentResolution.height}", DisplayerName);
            Fullscreen.DisplayValue(Screen.fullScreenMode.ToString(), DisplayerName);
            GPU.DisplayValue(SystemInfo.graphicsDeviceName, DisplayerName);
            CPU.DisplayValue(SystemInfo.processorType, DisplayerName);
            CPUThreads.DisplayValue(SystemInfo.processorCount.ToString(), DisplayerName);
            Memory.DisplayValue($"{SystemInfo.systemMemorySize}MB", DisplayerName);
            OS.DisplayValue(SystemInfo.operatingSystem.ToString(), DisplayerName);
        }

        private void Update()
        {
            DisplayFramerate();
            Resolution.DisplayValue($"{Screen.currentResolution.width}x{Screen.currentResolution.height}", DisplayerName);
            Fullscreen.DisplayValue(Screen.fullScreenMode.ToString(), DisplayerName);
        }

        private void DisplayFramerate()
        {
            time += Time.deltaTime;
            framecount++;
            if (time >= 1)
            {
                Framerate.DisplayValue($"{framecount} {time / framecount * 1000f}ms", DisplayerName);
                framecount = 0;
            }
            time %= 1;
        }
    }
}