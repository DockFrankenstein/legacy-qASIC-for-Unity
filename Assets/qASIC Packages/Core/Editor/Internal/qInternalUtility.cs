using System.Collections.Generic;
using UnityEditor;

namespace qASIC.Internal
{
    public static partial class qInternalUtility
    {
        public static void EnsureScriptDefineSymbols(HashSet<string> defines)
        {
            var previousDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var definesHash = new HashSet<string>(previousDefinesString.Split(';'));

            definesHash.UnionWith(defines);
            
            var newDefinesString = string.Join(";", definesHash);

            if (previousDefinesString != newDefinesString)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefinesString);
        }
    }
}