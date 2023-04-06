using System;
using System.Linq;

namespace qASIC.Input.Internal.KeyProviders
{
    public class GamepadButtonKeyProvider : KeyTypeProvider
    {
        public override string RootPath => "key_gamepad";
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
    }
}