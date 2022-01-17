using System;

namespace qASIC.InputManagement.Map.Internal
{
    public static class InputMapEditorUtility
    {
        public static string GenerateUniqueName(string name, Func<string, bool> condition)
        {
            int index;
            for (index = 0; condition.Invoke(GetUniqueIndexName(name, index)); index++) { }
            return GetUniqueIndexName(name, index);
        }

        private static string GetUniqueIndexName(string baseText, int index) =>
            index == 0 ? baseText : $"{baseText} {index - 1}";
    }
}