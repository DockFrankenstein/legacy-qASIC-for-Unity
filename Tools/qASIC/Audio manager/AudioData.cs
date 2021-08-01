﻿using UnityEngine;
using UnityEngine.Audio;

namespace qASIC.AudioManagment
{
    [System.Serializable]
    public struct AudioData
    {
        public AudioClip clip;
        public AudioMixerGroup group;
        public bool loop;
        public bool replace;

        public bool UseGlobalControls;

        public AudioData(AudioClip clip)
        {
            this.clip = clip;
            group = null;
            loop = false;
            replace = true;
            UseGlobalControls = true;
        }

        public AudioData(AudioClip clip, AudioMixerGroup group)
        {
            this.clip = clip;
            this.group = group;
            loop = false;
            replace = true;
            UseGlobalControls = true;
        }

        public AudioData(AudioClip clip, AudioMixerGroup group, bool loop, bool replace, bool useGlobalControls)
        {
            this.clip = clip;
            this.group = group;
            this.loop = loop;
            this.replace = replace;
            UseGlobalControls = useGlobalControls;
        }
    }
}