using UnityEngine;
using System;
using System.Linq;
using qASIC.Input.Devices;

namespace qASIC.Input.Internal.KeyProviders
{
    public class KeyCodeKeyProvider : KeyTypeProvider
    {
        public override string RootPath => "key_keyboard";
        public override string DisplayName => "Key";
        public override Type KeyType => typeof(KeyCode);

        public override string[] GetKeyList() =>
            _KeyNameMap
            .Select(x => x.Key)
            .ToArray();

        private static Map<string, KeyCode> _keyNameMap = null;
        static Map<string, KeyCode> _KeyNameMap
        {
            get
            {
                if (_keyNameMap == null)
                {
                    _keyNameMap = new Map<string, KeyCode>(UIMKeyboardProvider.AllKeyCodes
                        .ToDictionary(x => x.ToString()));
                }

                return _keyNameMap;
            }
        }
    }
}