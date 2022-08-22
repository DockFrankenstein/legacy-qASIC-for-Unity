#if UNITY_EDITOR
using System;
using UnityEngine;

namespace qASIC.InputManagement.Internal.KeyProviders
{
    public abstract class KeyTypeProvider
    {
        public abstract string KeyName { get; }
        public abstract Type KeyType { get; }

        public abstract int OnPopupGUI(Rect rect, int keyIndex, bool isActive, bool isFocused);

        public abstract int[] GetKeyList();
    }
}
#endif