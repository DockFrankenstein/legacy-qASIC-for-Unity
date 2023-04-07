using UnityEngine;
using System;
using System.Linq;

namespace qASIC
{
    public static class qApplication
    {
        public static RuntimePlatformFlags Platform { get; internal set; } = Application.platform.ToRuntimePlatformFlags();

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
    }
}