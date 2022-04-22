using UnityEngine;

namespace qASIC
{
    public static class qApplication
    {
        public static RuntimePlatform Platform { get; set; } = Application.platform;
    }
}