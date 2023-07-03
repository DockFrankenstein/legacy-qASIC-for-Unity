using qASIC.ProjectSettings;
using System.Collections.Generic;
using UnityEditor;

namespace qASIC.Internal
{
    internal static partial class AssetImporter
    {
        [InitializeOnLoadMethod]
        static void InitializeDisplayer()
        {
            _ = DisplayerProjectSettings.Instance;
            qInternalUtility.EnsureScriptDefineSymbols(new HashSet<string>()
            {
                "qASIC_INFO_DISPLAYER",
            });
        }
    }
}