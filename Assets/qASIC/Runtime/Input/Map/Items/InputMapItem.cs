using UnityEngine;
using System;
using qASIC.Tools;

namespace qASIC.Input.Map
{
    [Serializable] 
    public abstract class InputMapItem : INonRepeatable, IMapItem
    {
        public InputMapItem() { }

        public InputMapItem(string name)
        {
            itemName = name;
        }

        [SerializeField] string itemName;
        [SerializeField] string guid = System.Guid.NewGuid().ToString();

        [NonSerialized] internal InputMap map;

        /// <summary>Name of the item</summary>
        public string ItemName { get => itemName; set => itemName = value; }
        public string Guid { get => guid; set => guid = value; }
        public bool MapLoaded => map != null;


        public abstract Type ValueType { get; }

        public override string ToString() =>
            itemName;

        public bool NameEquals(string name) =>
            NonRepeatableChecker.Compare(itemName, name);

        public abstract object ReadValueAsObject(Func<string, float> func);
        public abstract object GetHighestValueAsObject(object a, object b);
        public abstract InputEventType GetInputEvent(Func<string, InputEventType> func);

        /// <summary>Checks if the item has any errors. This is used in the editor to signify if any changes are needed.</summary>
        public virtual bool HasErrors() =>
            false;
    }

    [Serializable]
    public abstract class InputMapItem<T> : InputMapItem
    {
        public InputMapItem() : base() { }
        public InputMapItem(string name) : base(name) { }

        public override Type ValueType => typeof(T);

        public override object ReadValueAsObject(Func<string, float> func) =>
            ReadValue(func);

        public override object GetHighestValueAsObject(object a, object b) =>
            GetHighestValue((T)a, (T)b);

        public abstract T ReadValue(Func<string, float> func);
        public abstract T GetHighestValue(T a, T b);
    }
}