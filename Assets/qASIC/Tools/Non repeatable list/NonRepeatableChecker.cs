using System.Collections.Generic;

namespace qASIC.Tools
{
    public static class NonRepeatableChecker
    {
        public static bool ContainsRepeatable<T>(List<T> list)
        {
            List<string> usedNames = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is INonRepeatable nonRepeatable) || usedNames.Contains(GetFormatedName(nonRepeatable.ItemName))) return true;
                usedNames.Add(GetFormatedName(nonRepeatable.ItemName));
            }

            return false;
        }

        public static bool ContainsKey<T>(List<T> list, string key)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] is INonRepeatable nonRepeatable && Compare(GetFormatedName(nonRepeatable.ItemName), key))
                    return true;

            return false;
        }

        public static bool LogContainsRepeatable<T>(List<T> list)
        {
            bool contains = ContainsRepeatable(list);

            if (contains)
                qDebug.LogError($"There are multiple items of the same name in a non repeatable list!");

            return contains;
        }

        public static void RemoveRepeatable<T>(ref List<T> list)
        {
            List<string> usedNames = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is INonRepeatable nonRepeatable) || usedNames.Contains(GetFormatedName(nonRepeatable.ItemName)))
                {
                    list.RemoveAt(i);
                    continue;
                }

                usedNames.Add(GetFormatedName(nonRepeatable.ItemName));
            }
        }

        public static List<string> GetNameList<T>(List<T> list)
        {
            List<string> names = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is INonRepeatable nonRepeatable)) continue;
                string name = GetFormatedName(nonRepeatable.ItemName);
                if (names.Contains(name)) continue;

                names.Add(name);
            }

            return names;
        }

        public static bool TryGetItem<T>(List<T> list, string itemName, out T item, bool logError = false)
        {
            itemName = GetFormatedName(itemName);
            item = default;

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is INonRepeatable nonRepeatable) || GetFormatedName(nonRepeatable.ItemName) != itemName)
                    continue;

                item = list[i];
                return true;
            }

            if (logError)
                qDebug.LogError($"Item '{itemName}' doesn't exist!");

            return false;
        }

        public static T GetItem<T>(List<T> list, string itemName)
        {
            TryGetItem(list, itemName, out T item);
            return item;
        }

        /// <summary>Compares two keys together</summary>
        /// <returns>Returns true if the keys are equal</returns>
        public static bool Compare(string key1, string key2) =>
            GetFormatedName(key1) == GetFormatedName(key2);

        public static string GetFormatedName(string name) =>
            name.ToLower();

        public static List<INonRepeatable> GenerateNonRepeatableList<T>(List<T> list)
        {
            List<INonRepeatable> nonRepeatableList = new List<INonRepeatable>();

            for (int i = 0; i < list.Count; i++)
                if (list[i] is INonRepeatable nonRepeatable)
                    nonRepeatableList.Add(nonRepeatable);

            return nonRepeatableList;
        }

        public static int IndexOf<T>(List<T> list, string itemName)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] is INonRepeatable nonRepeatable)
                    if (Compare(nonRepeatable.ItemName, itemName))
                        return i;

            return -1;
        }
    }
}