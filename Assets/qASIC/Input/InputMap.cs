using UnityEngine;
using System.Collections.Generic;
using qASIC.Tools;

namespace qASIC.InputManagement
{
    [CreateAssetMenu(fileName = "NewInputMap", menuName = "qASIC/Input/Input Map")]
    public class InputMap : ScriptableObject
    {
#if UNITY_EDITOR
        [HideInInspector] public int currentEditorSelectedGroup = -1;
        [HideInInspector] public bool autoSave = true;
#endif

        public int defaultGroup = 0;
        public List<InputGroup> Groups = new List<InputGroup>();

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
            group = null;

            for (int i = 0; i < Groups.Count; i++)
            {
                if (Groups[i].groupName != groupName) continue;
                group = Groups[i];
                return true;
            }

            if (logError)
                qDebug.LogError($"Map does not contain group <b>{groupName}</b>");

            return false;
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
    }
}