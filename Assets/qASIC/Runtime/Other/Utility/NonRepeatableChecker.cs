using qASIC;
using System.Collections.Generic;
using System.Linq;

namespace qASIC
{
    public static class NonRepeatableChecker
    {
        public static bool ContainsRepeatable<T>(List<T> list, bool logError = true)
        {
            var usedNames = GetNameEnumerable(list);
            bool contains = usedNames.Count() != usedNames.Distinct().Count();

            if (contains && logError)
                qDebug.LogError($"There are multiple items of the same name in a non repeatable list!");

            return contains;
        }

        public static bool ContainsKey<T>(List<T> list, string key) =>
            GetNameEnumerable(list).Contains(GetFormatedName(key));

        public static void RemoveRepeatable<T>(ref List<T> list) =>
            list = list
            .Where(x => x is INonRepeatable)
            .Select(x => x as INonRepeatable)
            .GroupBy(x => GetFormatedName(x.ItemName))
            .Select(x => (T)x.First())
            .ToList();

        public static List<string> GetNameList<T>(List<T> list) =>
            GetNameEnumerable(list)
            .ToList();

        public static IEnumerable<string> GetNameEnumerable<T>(List<T> list) =>
            list
            .Where(x => x is INonRepeatable)
            .Select(x => GetFormatedName((x as INonRepeatable).ItemName));

        public static bool TryGetItem<T>(List<T> list, string itemName, out T item, bool logError = false)
        {
            item = default;
            itemName = GetFormatedName(itemName);

            var targets = list
                .Where(x => x is INonRepeatable nonRepeatable && GetFormatedName(nonRepeatable.ItemName) == itemName);

            switch (targets.Count())
            {
                case 0:
                    if (logError)
                        qDebug.LogError($"Item '{itemName}' doesn't exist!");
                    return false;
                case 1:
                    item = targets.FirstOrDefault();
                    return true;
                default:
                    if (logError)
                        qDebug.LogError($"There are multiple items of name '{itemName}' in the list!");
                    return false;
            }
        }

        public static T GetItem<T>(List<T> list, string itemName)
        {
            TryGetItem(list, itemName, out var item);
            return item;
        }

        /// <summary>Compares two keys together</summary>
        /// <returns>Returns true if the keys are equal</returns>
        public static bool Compare(string key1, string key2) =>
            GetFormatedName(key1) == GetFormatedName(key2);

        public static string GetFormatedName(string name) =>
            name.ToLower();

        public static List<INonRepeatable> GenerateNonRepeatableList<T>(List<T> list) =>
            list
            .Where(x => x is INonRepeatable)
            .Select(x => x as INonRepeatable)
            .ToList();

        public static int IndexOf<T>(List<T> list, string itemName)
        {
            var nonRepeatableList = list
                .Select(x => (INonRepeatable)x)
                .ToList();

            for (int i = 0; i < nonRepeatableList.Count; i++)
                if (Compare(nonRepeatableList[i].ItemName, itemName))
                    return i;

            return -1;
        }
    }
}