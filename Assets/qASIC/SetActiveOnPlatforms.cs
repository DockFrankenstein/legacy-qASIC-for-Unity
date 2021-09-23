using System;
using UnityEngine;

namespace qASIC
{
    public class SetActiveOnPlatforms : MonoBehaviour
    {
        [SerializeField] bool state = false;
        [SerializeField] RuntimePlatform[] platforms = new RuntimePlatform[] { RuntimePlatform.WebGLPlayer };

        private void Awake()
        {
            gameObject.SetActive(Array.IndexOf(platforms, Application.platform) != -1 == state);
        }
    }
}