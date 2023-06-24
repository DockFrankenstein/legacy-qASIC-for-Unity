﻿using System.Collections.Generic;
using UnityEditor;
using qASIC.ProjectSettings;

namespace qASIC.Internal
{
    internal static partial class AssetImporter
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            _ = ConsoleProjectSettings.Instance;
            qInternalUtility.EnsureScriptDefineSymbols(new HashSet<string>()
            {
                "qASIC_CONSOLE",
            });
        }
    }
}