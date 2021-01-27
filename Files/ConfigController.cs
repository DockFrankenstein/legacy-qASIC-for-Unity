using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC.Console;

namespace qASIC.FileManaging
{
    public static class ConfigController
    {
        #region Sort
        /// <summary>Loads and saves specified config</summary>
        /// <returns>Returns the list of groups containing a list of keys</returns>
        public static List<List<string>> FixConfig(string path)
        {
            if (!FileManager.TryLoadTxtFile(path, out string data)) return new List<List<string>>();
            List<List<string>> values = Decode(data);
            FileManager.SaveTxtFile(path, Encode(values));
            return values;
        }

        /// <returns>Returns the trimmed value</returns>
        public static string SortValue(string value)
        {
            if (value.StartsWith(" "))
                return value.TrimStart(' ');
            return value;
        }
        #endregion

        #region Encode
        /// <param name="content">Loaded .txt files content</param>
        /// <returns>Returns the list of groups containing a list of keys</returns>
        public static List<List<string>> Decode(string content)
        {
            content = content.Replace('\r', '\n');
            string[] groups = content.Split('#');
            List<List<string>> data = new List<List<string>>();

            for (int x = 0; x < groups.Length; x++)
            {
                data.Add(new List<string>());
                List<string> temp = new List<string>(groups[x].Split('\n'));
                if (x != 0) temp[0] = $"#{temp[0]}";
                else temp.Insert(0, "#");
                for (int y = 0; y < temp.Count; y++)
                {
                    bool isEmpty = !((y != 0 || x != 0) && (temp[y] == "" || temp[y] == " "));
                    bool hasData = temp[y].Split(':').Length >= 2;

                    bool isGroupName = temp[y].StartsWith("#");

                    if (isEmpty && hasData)
                        data[x].Add($"{temp[y].Split(':')[0]}: {SortValue(temp[y].Split(':')[1])}");
                    else if (isGroupName)
                        data[x].Add(temp[y]);
                }
            }

            string test = "";
            for (int x = 0; x < data.Count; x++)
                for (int y = 0; y < data[x].Count; y++)
                    test += $"{data[x][y]}\n";

            return data;
        }

        /// <summary></summary>
        /// <param name="data">List of groups containing a list of keys</param>
        /// <returns>Returns converted list as a string</returns>
        public static string Encode(List<List<string>> data)
        {
            string result = "";
            for (int x = 0; x < data.Count; x++)
            {
                if (!(data[x].Count == 0 || (x == 0 && data[x].Count == 1)))
                {
                    for (int y = 0; y < data[x].Count; y++)
                    {
                        if (x == 0 && y == 0) y++;
                        result += $"{data[x][y]}\n";
                    }
                    result += "\n";
                }
            }
            return result;
        }
        #endregion

        #region GetValue
        public static string GetValue(string path, string settingName)
        { return GetValue(path, settingName, ""); }

        /// <param name="content">List of groups containing a list of keys</param>
        public static string GetValue(List<List<string>> content, string settingName)
        { return GetValue(content, settingName, ""); }

        public static string GetValue(string path, string settingName, string group)
        {
            if (TryGettingValue(path, settingName, group, out string value)) return value;
            GameConsoleController.Log("File does not exist!", "error");
            return "";
        }

        /// <param name="content">List of groups containing a list of keys</param>
        public static string GetValue(List<List<string>> content, string settingName, string group = "")
        {
            if (TryGettingValue(content, settingName, group, out string value)) return value;
            GameConsoleController.Log("File does not exist!", "error");
            return "";
        }

        public static bool TryGettingValue(string path, string settingName, out string value)
        { return TryGettingValue(path, settingName, "", out value); }

        /// <param name="content">List of groups containing a list of keys</param>
        public static bool TryGettingValue(List<List<string>> content, string settingName, out string value)
        { return TryGettingValue(content, settingName, "", out value); }

        public static bool TryGettingValue(string path, string settingName, string group, out string value)
        {
            value = "";
            if (FileManager.TryLoadTxtFile(path, out string config)) 
                return TryGettingValue(Decode(config), settingName, group, out value);
            return false;
        }

        /// <param name="content">List of groups containing a list of keys</param>
        public static bool TryGettingValue(List<List<string>> content, string settingName, string group, out string value)
        {
            value = "";
            if (TryGettingConfigGroup(group, content, out List<string> data))
                for (int i = 0; i < data.Count; i++)
                    if (data[i].Split(':').Length >= 2 && data[i].Split(':')[0] == settingName)
                    { value = SortValue(data[i].Split(':')[1]); return true; }
            return false;
        }
        #endregion

        #region Group
        /// <param name="content">List of groups containing a list of keys</param>
        /// <param name="result">If successful, returns the group containing keys</param>
        /// <returns>Returns if the group exists</returns>
        public static bool TryGettingConfigGroup(string groupName, List<List<string>> content, out List<string> result)
        {
            result = new List<string>();
            for (int i = 0; i < content.Count; i++)
                if (content[i][0] == $"#{groupName}") { result = content[i]; return true; }

            return false;
        }

        public static void DeleteGroup(string groupName, string path)
        {
            if (!FileManager.TryLoadTxtFile(path, out string data)) return;
            List<List<string>> config = Decode(data);
            for (int i = 0; i < config.Count; i++)
            {
                if (config[i][0] == $"#{groupName}")
                { config.RemoveAt(i); break; }
            }
            FileManager.SaveTxtFile(path, Encode(config));
        }

        /// <returns>Returns the list of groups containing a list of keys</returns>
        public static List<List<string>> AddGroup(string path, string groupName)
        { return AddGroup(FixConfig(path), groupName); }

        /// <param name="content">The list of groups containing a list of keys</param>
        /// <returns>Returns the new list of groups containing a list of keys</returns>
        public static List<List<string>> AddGroup(List<List<string>> content, string groupName)
        {
            bool groupExists = false;
            for (int i = 0; i < content.Count; i++)
                if (content[i][0] == $"#{groupName}")
                    groupExists = true;

            if (!groupExists)
                content.Add(new List<string>(new string[] { $"# + {groupName}" }));
            return content;
        }

        /// <returns>Returns the list of groups containing a list of keys</returns>
        private static List<List<string>> ReplaceGroup(List<List<string>> content, string groupName, List<string> newGroup)
        {
            for (int i = 0; i < content.Count; i++)
                if (content[i][0] == groupName)
                    content[i] = newGroup;
            return content;
        }
        #endregion

        #region Save
        /// <returns>Returns the new list of groups containing a list of keys</returns>
        public static List<List<string>> SaveSetting(string path, string settingName, string value)
        { return SaveSetting(path, settingName, value); }

        /// <returns>Returns the new list of groups containing a list of keys</returns>
        public static List<List<string>> SaveSetting(string path, string settingName, string value, string group)
        {
            List<List<string>> content = SaveSetting(FixConfig(path), settingName, value, group);
            FileManager.SaveTxtFile(path, Encode(content));
            return content;
        }

        /// <param name="content">The list of groups containing a list of keys</param>
        /// <returns>Returns the new list of groups containing a list of keys</returns>
        public static List<List<string>> SaveSetting(List<List<string>> content, string settingName, string value)
        { return SaveSetting(content, settingName, value, ""); }

        /// <param name="content">The list of groups containing a list of keys</param>
        /// <returns>Returns the new list of groups containing a list of keys</returns>
        public static List<List<string>> SaveSetting(List<List<string>> content, string settingName, string value, string group)
        {
            content = AddGroup(content, group);
            TryGettingConfigGroup(group, content, out List<string> groupData);
            bool settingExists = false;
            for (int i = 0; i < content.Count; i++)
                if (groupData[i].Split(':')[0] == settingName)
                { groupData[i] = $"{settingName}: {value}"; settingExists = true; break; }
            if (!settingExists) groupData.Add($"{settingName}: {value }");
            content = ReplaceGroup(content, group, groupData);
            return content;
        }
        #endregion

        /// <param name="content">List of groups containing a list of keys</param>
        public static bool KeyExistsInGroup(List<List<string>> content, string key, string group)
        {
            for (int x = 0; x < content.Count; x++)
                if (content[x][0] == $"#{group}")
                    for (int y = 0; y < content[x].Count; y++)
                        if (content[x][y].Split(':').Length >= 2)
                            if (content[x][y].Split(':')[0] == key)
                                return true;
            return false;
        }
    }
}