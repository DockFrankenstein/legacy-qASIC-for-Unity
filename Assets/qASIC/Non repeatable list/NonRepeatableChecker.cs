using System.Collections.Generic;

namespace qASIC
{
    public static class NonRepeatableChecker<T>
    {
        public static bool ContainsRepeatable(List<T> list)
        {
            List<string> usedNames = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is INonRepeatable nonRepeatable) || usedNames.Contains(nonRepeatable.ItemName.ToLower())) return true;
                usedNames.Add(nonRepeatable.ItemName.ToLower());
            }

            return false;
        }

        public static bool LogContainsRepeatable(List<T> list)
        {
            bool contains = ContainsRepeatable(list);

            if (contains)
                qDebug.LogError($"There are multiple items of the same name in a non repeatable list!");

            return contains;
        }

        public static void RemoveRepeatable(ref List<T> list)
        {
            List<string> usedNames = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is INonRepeatable nonRepeatable) || usedNames.Contains(nonRepeatable.ItemName.ToLower()))
                {
                    list.RemoveAt(i);
                    continue;
                }

                usedNames.Add(nonRepeatable.ItemName.ToLower());
            }
        }

        public static List<string> GetNameList(List<T> list)
        {
            List<string> names = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is INonRepeatable nonRepeatable) || names.Contains(nonRepeatable.ItemName.ToLower()))
                    continue;

                names.Add(nonRepeatable.ItemName.ToLower());
            }

            return names;
        }

        public static bool TryGetItem(List<T> list, string itemName, out T item, bool logError = false)
        {
            itemName = itemName.ToLower();
            item = default;

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is INonRepeatable nonRepeatable) || nonRepeatable.ItemName.ToLower() != itemName)
                    continue;

                item = list[i];
                return true;
            }

            if (logError)
                qDebug.LogError($"There are multiple items of name <b>{itemName}</b> in a non repeatable list!");

            return false;
        }

        public static T GetItem(List<T> list, string itemName)
        {
            TryGetItem(list, itemName, out T item);
            return item;
        }
    }
}