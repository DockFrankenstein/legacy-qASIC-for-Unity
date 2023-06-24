using UnityEngine;
using System;

namespace qASIC
{
    public class ObjectRequiresAttribute : PropertyAttribute
    {
        public Type[] RequiredTypes { get; }

        public ObjectRequiresAttribute(params Type[] type)
        {
            RequiredTypes = type;
        }
    }
}