#if UNITY_EDITOR
using System;
using System.Text.RegularExpressions;

namespace qASIC.InputManagement.Map.Internal
{
    public static class InputMapWindowEditorUtility
    {
        public static string GenerateUniqueName(string name, Func<string, bool> condition)
        {
            name = GetIndex(name, out int index);

            for (; condition.Invoke(GetUniqueIndexName(name, index)); index++) { }
            return GetUniqueIndexName(name, index);
        }

        private static string GetUniqueIndexName(string baseText, int index) =>
            index == 0 ? baseText : $"{baseText}{index - 1}";

        private static string GetIndex(string name, out int index)
        {
            index = 0;
            //Checks if there is a number at the end
            var m = Regex.Match(name, @"\d+$");
            if (!m.Success) return $"{name} ";

            //new index
            index = int.Parse(m.Value) + 1;
            //Removing the number
            name = name.Substring(0, name.LastIndexOf(m.Value));

            return name;
        }
    }
}
#endif