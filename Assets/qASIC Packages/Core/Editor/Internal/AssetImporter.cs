using System.Collections.Generic;
using UnityEditor;

namespace qASIC.Internal
{
    internal static partial class AssetImporter
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            qInternalUtility.EnsureScriptDefineSymbols(new HashSet<string>()
            {
                "qASIC_CORE",
            });
        }
    }
}