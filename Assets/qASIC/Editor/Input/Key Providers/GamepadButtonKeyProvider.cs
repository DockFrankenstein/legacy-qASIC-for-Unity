#if UNITY_EDITOR
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace qASIC.InputManagement.Internal.KeyProviders
{
    public class GamepadButtonKeyProvider : KeyTypeProvider
    {
        public override string KeyName => "key_gamepad";
        public override string DisplayName => "Gamepad Button";
        public override Type KeyType => typeof(GamepadButton);

        public override string[] GetKeyList() =>
            _KeyNameMap
            .Select(x => x.Key)
            .ToArray();

        private static Map<string, GamepadButton> _keyNameMap = null;
        static Map<string, GamepadButton> _KeyNameMap
        {
            get
            {
                if (_keyNameMap == null)
                {
                    _keyNameMap = new Map<string, GamepadButton>(((GamepadButton[])Enum.GetValues(typeof(GamepadButton)))
                        .ToDictionary(x => x.ToString()));
                }

                return _keyNameMap;
            }
        }

        public override string OnPopupGUI(Rect rect, string key, bool isActive, bool isFocused)
        {
            GamepadButton button = _KeyNameMap.Forward[key];
            return _KeyNameMap.Reverse[(GamepadButton)EditorGUI.EnumPopup(rect, string.Empty, button)];
        }
    }
}
#endif