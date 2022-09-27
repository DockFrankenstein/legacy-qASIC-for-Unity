#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using qASIC.InputManagement.Devices;
using UnityEditor;

namespace qASIC.InputManagement.Internal.KeyProviders
{
    public class KeyCodeKeyProvider : KeyTypeProvider
    {
        public override string KeyName => "key_keyboard";
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
                    _keyNameMap = new Map<string, KeyCode>(KeyboardManager.AllKeyCodes
                        .ToDictionary(x => x.ToString()));
                }

                return _keyNameMap;
            }
        }

        public override string OnPopupGUI(Rect rect, string key, bool isActive, bool isFocused)
        {
            KeyCode keyCode = _KeyNameMap.Forward[key];
            return _KeyNameMap.Reverse[(KeyCode)EditorGUI.EnumPopup(rect, string.Empty, keyCode)];
        }
    }
}
#endif