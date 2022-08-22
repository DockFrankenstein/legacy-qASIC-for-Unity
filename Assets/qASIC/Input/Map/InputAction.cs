using UnityEngine;
using System;
using System.Collections.Generic;
using qASIC.Tools;
using System.Linq;

namespace qASIC.InputManagement.Map
{
    [Serializable]
    public class InputAction : INonRepeatable
    {
        public InputAction() { }
        
        public InputAction(string name)
        {
            actionName = name;
        }

        public string actionName;
        public string guid = Guid.NewGuid().ToString();

        public List<KeyList> keys = new List<KeyList>();

        public string ItemName { get => actionName; set => actionName = value; }

        [Serializable]
        public struct KeyList
        {
            public KeyList(string keyType)
            {
                this.keyType = keyType;
                keys = new List<int>();
            }

            public KeyList(Type keyType) : this(keyType.AssemblyQualifiedName) { }

            public string keyType;
            public List<int> keys;
        }

        private Dictionary<Type, List<int>> keysCache = new Dictionary<Type, List<int>>();

        public List<int> GetKeyList(Type type)
        {
#if !UNITY_EDITOR && !QASIC_FORCE_CABLEBOX_INPUT_ACTION_EDITOR_KEY_CACHE
            if (keysCache.ContainsKey(type))
                return keysCache[type];
#endif

            string keyType = type.AssemblyQualifiedName;

            List<KeyList> targets = keys.Where(x => x.keyType == keyType).ToList();
            List<int> list = new List<int>();

            if (targets.Count == 1)
                list = keys.Where(x => x.keyType == keyType).ToList()[0].keys;

#if !UNITY_EDITOR && !QASIC_FORCE_CABLEBOX_INPUT_ACTION_EDITOR_KEY_CACHE
            if (!keysCache.ContainsKey(type))
                keysCache.Add(type, list);
#endif

            return list;
        }

        public InputAction Duplicate()
        {
            string json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<InputAction>(json);
        }

        public override string ToString() =>
            actionName;

        public bool NameEquals(string name) =>
            NonRepeatableChecker.Compare(actionName, name);
    }
}