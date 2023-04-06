using System;

namespace qASIC.Input.Internal.KeyProviders
{
    public abstract class KeyTypeProvider
    {
        public abstract string RootPath { get; }
        public abstract string DisplayName { get; }
        public abstract Type KeyType { get; }

        public abstract string[] GetKeyList();
    }
}