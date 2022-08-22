#if UNITY_EDITOR
using UnityEngine;
using System;
using UnityEditor;

namespace qASIC.InputManagement.Internal.KeyProviders
{
    public class GamepadButtonKeyProvider : KeyTypeProvider
    {
        public override string KeyName => "Gamepad Button";
        public override Type KeyType => typeof(GamepadButton);

        public override int[] GetKeyList() =>
            (int[])Enum.GetValues(KeyType);

        public override int OnPopupGUI(Rect rect, int keyIndex, bool isActive, bool isFocused)
        {
            GamepadButton button = (GamepadButton)keyIndex;
            return (int)(GamepadButton)EditorGUI.EnumPopup(rect, string.Empty, button);
        }
    }
}
#endif