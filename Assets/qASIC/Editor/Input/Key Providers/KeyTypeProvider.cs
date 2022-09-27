#if UNITY_EDITOR
using System;
using UnityEngine;

namespace qASIC.InputManagement.Internal.KeyProviders
{
    public abstract class KeyTypeProvider
    {
        public abstract string KeyName { get; }
        public abstract string DisplayName { get; }
        public abstract Type KeyType { get; }

        public abstract string OnPopupGUI(Rect rect, string key, bool isActive, bool isFocused);

        public abstract string[] GetKeyList();
    }
}
#endif