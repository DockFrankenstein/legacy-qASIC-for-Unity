using UnityEngine;
using System;

namespace qASIC
{
    /// <summary>Overrides inspector label</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InspectorLabelAttribute : PropertyAttribute
    {
        public string Label { get; }

        public InspectorLabelAttribute(string label)
        {
            Label = label;
        }
    }
}
