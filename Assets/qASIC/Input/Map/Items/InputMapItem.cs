using UnityEngine;
using System;
using qASIC.Tools;
using qASIC.InputManagement.Devices;

namespace qASIC.InputManagement.Map
{
    [Serializable] 
    public abstract class InputMapItem : INonRepeatable, ICloneable
    {
        public InputMapItem() { }

        public InputMapItem(string name)
        {
            itemName = name;
        }

        public string itemName;
        public string guid = Guid.NewGuid().ToString();

        [NonSerialized] protected InputMap map;

        public string ItemName 
        { 
            get => itemName;
            set => itemName = value;
        }

        public abstract Type ValueType { get; }

        public override string ToString() =>
            itemName;

        object ICloneable.Clone() =>
            Clone();

        public virtual InputMapItem Clone()
        {
            string json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<InputMapItem>(json);
        }

        public bool NameEquals(string name) =>
            NonRepeatableChecker.Compare(itemName, name);

        public abstract object ReadValueAsObject(Func<string, float> func);
        public abstract object GetHighestValueAsObject(object a, object b);
        public abstract bool GetInputEvent(Func<string, bool> func);
    }

    [Serializable]
    public abstract class InputMapItem<T> : InputMapItem
    {
        public InputMapItem() : base() { }
        public InputMapItem(string name) : base(name) { }

        public override Type ValueType => typeof(T);

        public override object ReadValueAsObject(Func<string, float> func) =>
            ReadValue(func);

        public override object GetHighestValueAsObject(object a, object b)
        {
            T x = (T)a;
            T y = (T)b;
            return GetHighestValue(x, y);
        }

        public abstract T ReadValue(Func<string, float> func);
        public abstract T GetHighestValue(T a, T b);
    }
}