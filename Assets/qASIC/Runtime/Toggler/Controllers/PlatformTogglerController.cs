﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace qASIC.Toggling.Controllers
{
    public class PlatformTogglerController : TogglerController
    {
        [SerializeField] Toggler defaultToggler;
        [SerializeField] TargetToggler[] platformTogglers = new TargetToggler[0];

        [Serializable]
        public struct TargetToggler
        {
            public RuntimePlatform platform;
            public Toggler toggler;
        }

        private void Awake()
        {
            for (int i = 0; i < platformTogglers.Length; i++)
            {
                if (platformTogglers == null) continue;
                if (platformTogglers[i].platform != qApplication.Platform) continue;
                ChangeToggler(platformTogglers[i].toggler);
                return;
            }

            ChangeToggler(defaultToggler);
        }

        protected override void HandleInput()
        {
            CurrentToggler?.ForceInputHandle();
        }

        public void FixPlatformTogglers()
        {
            Dictionary<RuntimePlatform, Toggler> togglers = new Dictionary<RuntimePlatform, Toggler>();

            for (int i = 0; i < platformTogglers.Length; i++)
                if (!togglers.ContainsKey(platformTogglers[i].platform))
                    togglers.Add(platformTogglers[i].platform, platformTogglers[i].toggler);

            RuntimePlatform[] platforms = qApplication.UnitySupportedPlatforms;

            int platformsCount = platforms.Length;

            platformTogglers = new TargetToggler[platformsCount];

            for (int i = 0; i < platformsCount; i++)
            {
                platformTogglers[i] = new TargetToggler()
                {
                    platform = platforms[i],
                    toggler = togglers.ContainsKey(platforms[i]) ? togglers[platforms[i]] : null,
                };
            }
        }
    }
}