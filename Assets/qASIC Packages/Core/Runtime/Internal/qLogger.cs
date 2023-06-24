using UnityEngine;
using System;

namespace qASIC.Internal
{
    public static class qLogger
    {
        public static Action<string, Color> OnLogColor;
        public static Action<string, string> OnLogColorTag;
        public static Action<string, Color> OnLogColorInternal;
        public static Action<string, string> OnLogColorTagInternal;
    }
}