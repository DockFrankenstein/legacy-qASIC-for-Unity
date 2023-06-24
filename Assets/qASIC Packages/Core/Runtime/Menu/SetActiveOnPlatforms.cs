using System;
using UnityEngine;

namespace qASIC
{
    [AddComponentMenu("qASIC/Menu/Set Active On Platforms")]
    public class SetActiveOnPlatforms : MonoBehaviour
    {
        [SerializeField] bool state = false;
        [SerializeField] RuntimePlatformFlags platform = RuntimePlatformFlags.WebGLPlayer;

        private void Awake()
        {
            gameObject.SetActive(platform.HasFlag(qApplication.Platform) == state);
        }
    }
}