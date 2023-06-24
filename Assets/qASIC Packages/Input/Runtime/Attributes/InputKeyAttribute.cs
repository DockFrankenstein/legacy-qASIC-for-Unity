using UnityEngine;
using System;

namespace qASIC.Input
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InputKeyAttribute : PropertyAttribute
    {
        public InputKeyAttribute() { }

        public InputKeyAttribute(string rootPath)
        {
            RootPath = rootPath;
        }

        public string RootPath { get; private set; } = string.Empty;
    }
}