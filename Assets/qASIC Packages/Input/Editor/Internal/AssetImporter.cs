using qASIC.Input.Devices;
using qASIC.Input.Map;
using qASIC.ProjectSettings;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace qASIC.Internal
{
    internal static partial class AssetImporter
    {
#if !qASIC_INPUT
        [InitializeOnLoadMethod]
        static void InitializeInput()
        {
            //Create settings
            var settings = InputProjectSettings.Instance;

            //Create map and structure
            var map = ScriptableObject.CreateInstance<InputMap>();
            AssetDatabase.CreateAsset(map, "Assets/Cablebox Input Map.asset");
            settings.map = map;

            var deviceStructure = ScriptableObject.CreateInstance<DeviceStructure>();
            AssetDatabase.CreateAsset(deviceStructure, "Assets/Cablebox Device Structure.asset");

            deviceStructure.AddHandler(typeof(UIMKeyboardProvider));
            deviceStructure.AddHandler(typeof(XInputGamepadProvider));
            settings.deviceStructure = deviceStructure;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            qInternalUtility.EnsureScriptDefineSymbols(new HashSet<string>()
            {
                "qASIC_INPUT",
            });
        }
#endif
    }
}