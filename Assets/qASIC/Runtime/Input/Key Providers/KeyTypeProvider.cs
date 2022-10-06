#if UNITY_EDITOR
using System;
using UnityEngine;

namespace qASIC.Input.Internal.KeyProviders
{
    public abstract class KeyTypeProvider
    {
        public abstract string KeyName { get; }
        public abstract string DisplayName { get; }
        public abstract Type KeyType { get; }

        public abstract string[] GetKeyList();
    }
}
#endif