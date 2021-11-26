using UnityEngine;
using System.Collections.Generic;

namespace qASIC.InputManagement
{
    [CreateAssetMenu(fileName = "NewInputMap", menuName = "qASIC/Input/Input Map")]
    public class InputMap : ScriptableObject
    {
#if UNITY_EDITOR
        public int currentEditorSelectedGroup = -1;
#endif

        public string defaultGroup = "Game";

        public List<InputGroup> Groups = new List<InputGroup>();

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
        public void CheckForRepeatingGroups()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < Groups.Count; i++)
            {
                if (names.Contains(Groups[i].groupName))
                {
                    qDebug.LogError($"There are multiple groups in the map, cannot index group <b>{Groups[i].groupName}</b>");
                    continue;
                }

                names.Add(Groups[i].groupName);
            }
        }
    }
}