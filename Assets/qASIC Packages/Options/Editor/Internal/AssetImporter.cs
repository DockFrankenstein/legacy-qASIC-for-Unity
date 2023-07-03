using qASIC.ProjectSettings;
using System.Collections.Generic;
using UnityEditor;

namespace qASIC.Internal
{
    internal static partial class AssetImporter
    {
        [InitializeOnLoadMethod]
        static void InitializeOptions()
        {
            _ = OptionsProjectSettings.Instance;
            qInternalUtility.EnsureScriptDefineSymbols(new HashSet<string>()
            {
                "qASIC_OPTIONS",
            });
        }
    }
}