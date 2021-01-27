using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC.Console;

namespace qASIC.FileManaging
{
    public static class ConfigController
    {
        public static List<List<string>> FixConfig(string path)
        {
            if (FileManager.TryLoadTxtFile(path, out string data))
            {
                List<List<string>> values = Decode(data);
                FileManager.SaveTxtFile(path, Encode(values));
                return values;
            }
            return new List<List<string>>();
        }

        public static void DeleteGroup(string groupName, string path)
        {
            if (FileManager.TryLoadTxtFile(path, out string data))
            {
                List<List<string>> config = Decode(data);
                for (int i = 0; i < config.Count; i++)
                {
                    if (config[i][0] == $"#{groupName}")
                    { config.RemoveAt(i); break; }
                }
                FileManager.SaveTxtFile(path, Encode(config));
            }
        }

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

        public static bool OptionExistsInGroup(List<List<string>> content, string option, string group)
        {
            for (int x = 0; x < content.Count; x++)
                if (content[x][0] == $"#{group}")
                    for (int y = 0; y < content[x].Count; y++)
                        if (content[x][y].Split(':').Length >= 2)
                            if (content[x][y].Split(':')[0] == option)
                                return true;
            return false;
        }

        public static string SortValue(string value)
        {
            if (value.StartsWith(" "))
                return value.TrimStart(' ');
            return value;
        }

        public static bool TryGettingConfigGroup(string groupName, List<List<string>> content, out List<string> result)
        {
            result = new List<string>();
            for (int i = 0; i < content.Count; i++)
                if (content[i][0] == $"#{groupName}") { result = content[i]; return true; }

            return false;
        }

        public static string GetSetting(string path, string settingName, string group = "")
        {
            if (TryGettingSetting(path, settingName, group, out string value))
                return value;
            GameConsoleController.Log("File does not exist!", "error");
            return "";
        }

        public static bool TryGettingSetting(string path, string settingName, out string value)
        {
            return TryGettingSetting(path, settingName, "", out value);
        }

        public static bool TryGettingSetting(string path, string settingName, string group, out string value)
        {
            value = "";
            if (FileManager.TryLoadTxtFile(path, out string config))
                if (TryGettingConfigGroup(group, Decode(config), out List<string> data))
                    for (int i = 0; i < data.Count; i++)
                        if (data[i].Split(':')[0] == settingName)
                        { value = SortValue(data[i].Split(':')[1]); return true; }

            return false;
        }

        public static List<List<string>> AddGroup(string path, string groupName)
        {
            List<List<string>> config = FixConfig(path);

            bool groupExists = false;
            for (int i = 0; i < config.Count; i++)
                if (config[i][0] == $"#{groupName}")
                    groupExists = true;

            if (!groupExists)
                config.Add(new List<string>(new string[] { $"# + {groupName}" }));
            return config;
        }

        private static List<List<string>> ReplaceGroup(List<List<string>> content, string groupName, List<string> newGroup)
        {
            for (int i = 0; i < content.Count; i++)
                if (content[i][0] == groupName)
                    content[i] = newGroup;
            return content;
        }

        public static void SaveSetting(string path, string settingName, string value, string group = "")
        {
            List<List<string>> config = AddGroup(path, group);
            TryGettingConfigGroup(group, config, out List<string> content);

            bool settingExists = false;
            for (int i = 0; i < content.Count; i++)
                if (content[i].Split(':')[0] == settingName)
                { content[i] = $"{settingName}: {value}"; settingExists = true; break; }

            if (!settingExists)
                content.Add($"{settingName}: {value }");

            config = ReplaceGroup(config, group, content);
            FileManager.SaveTxtFile(path, Encode(config));
            FixConfig(path);
        }
    }
}