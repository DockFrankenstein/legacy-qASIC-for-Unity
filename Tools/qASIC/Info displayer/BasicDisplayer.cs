using UnityEngine;

namespace qASIC.Displayer.Displayers
{
    public class BasicDisplayer : MonoBehaviour
    {
        public string DisplayerName = "main";

        [Header("Settings")]
        public DisplayerValueAssigner Framerate = new DisplayerValueAssigner("fps");
        public DisplayerValueAssigner Resolution = new DisplayerValueAssigner("resolution");
        public DisplayerValueAssigner Fullscreen = new DisplayerValueAssigner("fullscreen");
        public DisplayerValueAssigner GPU = new DisplayerValueAssigner("gpu");
        public DisplayerValueAssigner CPU = new DisplayerValueAssigner("cpu");
        public DisplayerValueAssigner CPUThreads = new DisplayerValueAssigner("cpu threads");
        public DisplayerValueAssigner Memory = new DisplayerValueAssigner("memory");
        public DisplayerValueAssigner OS = new DisplayerValueAssigner("os");


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

        private void Reset()
        {
            InfoDisplayer infoDisplayer = GetComponent<InfoDisplayer>();
            if (infoDisplayer != null) DisplayerName = infoDisplayer.DisplayerName;
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