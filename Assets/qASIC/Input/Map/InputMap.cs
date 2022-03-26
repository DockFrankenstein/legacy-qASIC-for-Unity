using UnityEngine;
using System.Collections.Generic;
using qASIC.Tools;

namespace qASIC.InputManagement.Map
{
    [CreateAssetMenu(fileName = "NewInputMap", menuName = "qASIC/Input/Input Map")]
    public class InputMap : ScriptableObject
    {
        public int defaultGroup = 0;
        public List<InputGroup> Groups = new List<InputGroup>(new InputGroup[] { new InputGroup("Default") });

        public string DefaultGroupName
        { 
            get
            {
                if (defaultGroup >= 0 && defaultGroup < Groups.Count)
                    return Groups[defaultGroup].groupName;
                return string.Empty;
            }
        }

        public bool TryGetGroup(string groupName, out InputGroup group, bool logError = false)
        {
            bool contains = NonRepeatableChecker.TryGetItem(Groups, groupName, out group);

            if (!contains && logError)
                qDebug.LogError($"Map does not contain group '{groupName}'");

            return contains;
        }

        public InputGroup GetGroup(string groupName)
        {
            TryGetGroup(groupName, out InputGroup group, true);
            return group;
        }

        /// <summary>Checks if there are no duplicate groups</summary>
        public void CheckForRepeating() =>
            NonRepeatableChecker.LogContainsRepeatable(Groups);

        public string[] GetGroupNames()
        {
            string[] names = new string[Groups.Count];
            for (int i = 0; i < Groups.Count; i++)
                names[i] = Groups[i].groupName;

            return names;
        }

        public bool GroupExists(string groupName)
        {
            for (int i = 0; i < Groups.Count; i++)
                if (Groups[i]?.NameEquals(groupName) == true)
                    return true;
            return false;
        }

        public bool CanRenameGroup(string newName) =>
            !string.IsNullOrWhiteSpace(newName) && !GroupExists(newName);
    }
}