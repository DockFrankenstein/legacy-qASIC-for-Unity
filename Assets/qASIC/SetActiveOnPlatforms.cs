using System;
using UnityEngine;

namespace qASIC
{
    [AddComponentMenu("qASIC/Menu/Set Active On Platforms")]
    public class SetActiveOnPlatforms : MonoBehaviour
    {
        [SerializeField] bool state = false;
        [SerializeField] RuntimePlatform[] platforms = new RuntimePlatform[] { RuntimePlatform.WebGLPlayer };

        private void Awake()
        {
            gameObject.SetActive(Array.IndexOf(platforms, qApplication.Platform) != -1 == state);
        }
    }
}