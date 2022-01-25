﻿using UnityEngine;

namespace qASIC.ProjectSettings
{
    [System.Serializable]
    [ExcludeFromPreset]
    public class DisplayerProjectSettings : ProjectSettingsBase
    {
        private static DisplayerProjectSettings _instance;
        public static DisplayerProjectSettings Instance { get => CheckInstance("Displayer", _instance); }


        public string debugObjectName = "Debug Displayer - autogenerated";
        public string debugDisplayerName = "debug";

        [Header("Toggler")]
        public string debugTogglerName = "debug displayer";
        [KeyCodeListener] public KeyCode debugTogglerKey = KeyCode.None;

        [Header("Auto generation console message")]
        public bool displayDebugGenerationMessage = true;
        [TextArea(3, 5)]
        public string debugGenerationMessage = "Generated debug displayer. This only happens in the editor and dev build!";
        public string debugGenerationMessageColor = "warning";

        [Header("Auto generation")]
        public bool createDebugInEditor = true;
        public bool createDebugInDeveloperBuild = true;
        public bool createDebugInBuild = false;

        public bool CreateDebugDisplayer
        {
            get
            {
#if UNITY_EDITOR
                return createDebugInEditor;
#else            
                return (Debug.isDebugBuild && createDebugInDeveloperBuild) || createDebugInBuild;
#endif

            }
        }
    }
}