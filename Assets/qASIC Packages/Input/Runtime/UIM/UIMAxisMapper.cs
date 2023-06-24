using qASIC.Input.Devices;
using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Input.UIM
{
    [CreateAssetMenu(fileName = "NewUIMAxisMapper", menuName = "qASIC/Input/UIM Axis Mapper", order = 20)]
    public class UIMAxisMapper : ScriptableObject
    {
        public UIMAxisMapperPlatform defaultMapper;
        public UIMAxisMapperPlatform windowsMapper;
        public UIMAxisMapperPlatform macMapper;
        public UIMAxisMapperPlatform linuxMapper;

        private static UIMAxisMapperPlatform _currentMapper = null;
        public static UIMAxisMapperPlatform CurrentMapper
        {
            get
            {
                UIMAxisMapper mapper = UIMGamepadProvider.AxisMapper;
                if (mapper == null)
                    return null;

                if (_currentMapper == null)
                {
                    switch (SystemInfo.operatingSystemFamily)
                    {
                        case OperatingSystemFamily.Windows:
                            _currentMapper = mapper.windowsMapper;
                            break;
                        case OperatingSystemFamily.Linux:
                            _currentMapper = mapper.linuxMapper;
                            break;
                        case OperatingSystemFamily.MacOSX:
                            _currentMapper = mapper.macMapper;
                            break;
                    }

                    if (_currentMapper == null)
                        _currentMapper = mapper.defaultMapper;
                }

                return _currentMapper;
            }
        }

        private static Dictionary<GamepadButton, UIMAxisMapperPlatform.ButtonMapping> _axisDictionary = null;
        private static Dictionary<GamepadButton, UIMAxisMapperPlatform.ButtonMapping> AxisDictionary
        {
            get
            {
                if (CurrentMapper == null)
                    return null;

                if (_axisDictionary == null)
                {
                    _axisDictionary = new Dictionary<GamepadButton, UIMAxisMapperPlatform.ButtonMapping>();

                    foreach (var axis in CurrentMapper.axes)
                    {
                        if (_axisDictionary.ContainsKey(axis.button))
                            continue;

                        _axisDictionary.Add(axis.button, axis);
                    }

                }

                return _axisDictionary;
            }
        }

        public static UIMAxisMapperPlatform.ButtonMapping GetButtonMapping(GamepadButton button)
        {
            if (!AxisDictionary.TryGetValue(button, out UIMAxisMapperPlatform.ButtonMapping item))
                return new UIMAxisMapperPlatform.ButtonMapping();

            return item;
        }
    }
}