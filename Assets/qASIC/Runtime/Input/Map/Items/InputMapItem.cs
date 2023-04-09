using UnityEngine;
using System;

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
        public virtual Color ItemColor => qASIC.Internal.Info.qASICColor;


        public abstract Type ValueType { get; }

        public override string ToString() =>
            itemName;

        public bool NameEquals(string name) =>
            NonRepeatableChecker.Compare(itemName, name);

        public virtual void OnCreated() { }

        public abstract object ReadValueAsObject(InputMapData data, Func<string, float> func);
        public abstract InputEventType GetInputEvent(InputMapData data, Func<string, InputEventType> func);
        public abstract object GetHighestValueAsObject(object a, object b);

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

        public override object ReadValueAsObject(InputMapData data, Func<string, float> func) =>
            ReadValue(data, func);

        public abstract T ReadValue(InputMapData data, Func<string, float> func);


        public override object GetHighestValueAsObject(object a, object b) =>
            GetHighestValue((T)a, (T)b);
        public abstract T GetHighestValue(T a, T b);
    }
}