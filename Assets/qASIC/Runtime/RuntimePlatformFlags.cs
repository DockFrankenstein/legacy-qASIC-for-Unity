using System;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.PlayerSettings;
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

    public static class RuntimePlatformExtensions
    {
        public static RuntimePlatformFlags ToRuntimePlatformFlags(this RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.OSXEditor:
                    return RuntimePlatformFlags.OSXEditor;
                case RuntimePlatform.OSXPlayer:
                    return RuntimePlatformFlags.OSXPlayer;
                case RuntimePlatform.WindowsPlayer:
                    return RuntimePlatformFlags.WindowsPlayer;
                case RuntimePlatform.WindowsEditor:
                    return RuntimePlatformFlags.WindowsEditor;
                case RuntimePlatform.IPhonePlayer:
                    return RuntimePlatformFlags.IPhonePlayer;
                case RuntimePlatform.Android:
                    return RuntimePlatformFlags.Android;
                case RuntimePlatform.LinuxPlayer:
                    return RuntimePlatformFlags.LinuxPlayer;
                case RuntimePlatform.LinuxEditor:
                    return RuntimePlatformFlags.LinuxEditor;
                case RuntimePlatform.WebGLPlayer:
                    return RuntimePlatformFlags.WebGLPlayer;
                case RuntimePlatform.WSAPlayerX86:
                    return RuntimePlatformFlags.WSAPlayerX86;
                case RuntimePlatform.WSAPlayerX64:
                    return RuntimePlatformFlags.WSAPlayerX64;
                case RuntimePlatform.WSAPlayerARM:
                    return RuntimePlatformFlags.WSAPlayerARM;
                case RuntimePlatform.PS4:
                    return RuntimePlatformFlags.PS4;
                case RuntimePlatform.XboxOne:
                    return RuntimePlatformFlags.XboxOne;
                case RuntimePlatform.tvOS:
                    return RuntimePlatformFlags.tvOS;
                case RuntimePlatform.Switch:
                    return RuntimePlatformFlags.Switch;
                case RuntimePlatform.Lumin:
                    return RuntimePlatformFlags.Lumin;
                case RuntimePlatform.Stadia:
                    return RuntimePlatformFlags.Stadia;
                case RuntimePlatform.CloudRendering:
                    return RuntimePlatformFlags.CloudRendering;
                case RuntimePlatform.GameCoreXboxSeries:
                    return RuntimePlatformFlags.GameCoreXboxSeries;
                case RuntimePlatform.GameCoreXboxOne:
                    return RuntimePlatformFlags.GameCoreXboxOne;
                case RuntimePlatform.PS5:
                    return RuntimePlatformFlags.PS5;
                default:
                    return RuntimePlatformFlags.None;
            }
        }

        public static List<RuntimePlatform> ToRuntimePlatform(this RuntimePlatformFlags flags)
        {
            List<RuntimePlatform> list = new List<RuntimePlatform>();
            var values = (RuntimePlatform[])Enum.GetValues(typeof(RuntimePlatform));

            foreach (var item in values)
            {
                var platform = item.ToRuntimePlatformFlags();
                if (platform == RuntimePlatformFlags.None) continue;

                if (flags.HasFlag(platform))
                    list.Add(item);
            }

            return list;
        }
    }
}
