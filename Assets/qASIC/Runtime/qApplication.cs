using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace qASIC
{
    [Flags]
    public enum RuntimePlatformFlags
    {
        None = 0,
        Everything = 4194303,
        OSXEditor = 1,
        OSXPlayer = 2,
        WindowsPlayer = 4,
        WindowsEditor = 8,
        IPhonePlayer = 16,
        Android = 32,
        LinuxPlayer = 64,
        LinuxEditor = 128,
        WebGLPlayer = 256,
        WSAPlayerX86 = 512,
        WSAPlayerX64 = 1024,
        WSAPlayerARM = 2048,
        PS4 = 4096,
        XboxOne = 8192,
        tvOS = 16384,
        Switch = 32768,
        Lumin = 65536,
        Stadia = 131072,
        CloudRendering = 262144,
        GameCoreXboxSeries = 524288,
        GameCoreXboxOne = 1048576,
        PS5 = 2097152,
    }

    public static class qApplication
    {
        public static RuntimePlatform Platform { get; internal set; } = Application.platform;

        private static RuntimePlatform[] _unitySupportedPlatforms = null;
        public static RuntimePlatform[] UnitySupportedPlatforms
        {
            get
            {
                if (_unitySupportedPlatforms == null)
                {
                    _unitySupportedPlatforms = ((RuntimePlatform[])Enum.GetValues(typeof(RuntimePlatform)))
                        .Distinct()
                        .Where(x =>
                        {
                            var attributes = (ObsoleteAttribute[])typeof(RuntimePlatform)
                            .GetField(x.ToString())
                            .GetCustomAttributes(typeof(ObsoleteAttribute), false);
                            return attributes == null || attributes.Length == 0;
                        }).ToArray();
                }

                return _unitySupportedPlatforms;
            }
        }

        public static bool FlagsContainPlatform(RuntimePlatformFlags flags, RuntimePlatform platform)
        {
            RuntimePlatformFlags platformFlag;
            switch (platform)
            {
                case RuntimePlatform.OSXEditor:
                    platformFlag = RuntimePlatformFlags.OSXEditor;
                    break;
                case RuntimePlatform.OSXPlayer:
                    platformFlag = RuntimePlatformFlags.OSXPlayer;
                    break;
                case RuntimePlatform.WindowsPlayer:
                    platformFlag = RuntimePlatformFlags.WindowsPlayer;
                    break;
                case RuntimePlatform.WindowsEditor:
                    platformFlag = RuntimePlatformFlags.WindowsEditor;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platformFlag = RuntimePlatformFlags.IPhonePlayer;
                    break;
                case RuntimePlatform.Android:
                    platformFlag = RuntimePlatformFlags.Android;
                    break;
                case RuntimePlatform.LinuxPlayer:
                    platformFlag = RuntimePlatformFlags.LinuxPlayer;
                    break;
                case RuntimePlatform.LinuxEditor:
                    platformFlag = RuntimePlatformFlags.LinuxEditor;
                    break;
                case RuntimePlatform.WebGLPlayer:
                    platformFlag = RuntimePlatformFlags.WebGLPlayer;
                    break;
                case RuntimePlatform.WSAPlayerX86:
                    platformFlag = RuntimePlatformFlags.WSAPlayerX86;
                    break;
                case RuntimePlatform.WSAPlayerX64:
                    platformFlag = RuntimePlatformFlags.WSAPlayerX64;
                    break;
                case RuntimePlatform.WSAPlayerARM:
                    platformFlag = RuntimePlatformFlags.WSAPlayerARM;
                    break;
                case RuntimePlatform.PS4:
                    platformFlag = RuntimePlatformFlags.PS4;
                    break;
                case RuntimePlatform.XboxOne:
                    platformFlag = RuntimePlatformFlags.XboxOne;
                    break;
                case RuntimePlatform.tvOS:
                    platformFlag = RuntimePlatformFlags.tvOS;
                    break;
                case RuntimePlatform.Switch:
                    platformFlag = RuntimePlatformFlags.Switch;
                    break;
                case RuntimePlatform.Lumin:
                    platformFlag = RuntimePlatformFlags.Lumin;
                    break;
                case RuntimePlatform.Stadia:
                    platformFlag = RuntimePlatformFlags.Stadia;
                    break;
                case RuntimePlatform.CloudRendering:
                    platformFlag = RuntimePlatformFlags.CloudRendering;
                    break;
                case RuntimePlatform.GameCoreXboxSeries:
                    platformFlag = RuntimePlatformFlags.GameCoreXboxSeries;
                    break;
                case RuntimePlatform.GameCoreXboxOne:
                    platformFlag = RuntimePlatformFlags.GameCoreXboxOne;
                    break;
                case RuntimePlatform.PS5:
                    platformFlag = RuntimePlatformFlags.PS5;
                    break;
                default:
                    platformFlag = RuntimePlatformFlags.None;
                    break;
            }

            return flags.HasFlag(platformFlag);
        }

        public static List<RuntimePlatform> GetPlatformsFromFlags(RuntimePlatformFlags flags)
        {
            List<RuntimePlatform> list = new List<RuntimePlatform>();
            var supportedPlatforms = UnitySupportedPlatforms;

            foreach (var item in supportedPlatforms)
                if (FlagsContainPlatform(flags, item))
                    list.Add(item);

            return list;
        }
    }
}