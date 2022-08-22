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
        public override string KeyName => "Key";
        public override Type KeyType => typeof(KeyCode);

        public override int[] GetKeyList() =>
            KeyboardManager.AllKeyCodes
                .Select(x => (int)x)
                .ToArray();

        public override int OnPopupGUI(Rect rect, int keyIndex, bool isActive, bool isFocused)
        {
            KeyCode keyCode = (KeyCode)keyIndex;
            return (int)(KeyCode)EditorGUI.EnumPopup(rect, string.Empty, keyCode);
        }
    }
}
#endif