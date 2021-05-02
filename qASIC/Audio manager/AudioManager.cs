﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using qASIC.FileManagement;
using qASIC.Console;

namespace qASIC.AudioManagment
{
    public class AudioManager : MonoBehaviour
    {
        public AudioMixer Mixer;
        public bool RoundValue = true;

        [Header("Saving")]
        public TextAsset SaveFilePreset;
        public string UserSavePath = "qASIC/Audio.txt";
        public string EditorUserSavePath = "qASIC/Audio-editor.txt";
        private static string _config = string.Empty;

        #region Singleton
        private static AudioManager singleton;

        private void Awake() => AssignSingleton();

        private static void CheckSingleton()
        {
            if (singleton != null) return;
            GameConsoleController.Log("Audio manager does not exist! Instantiating...", "error");
            new GameObject("Audio Manager - autogenerated").AddComponent<AudioManager>().AssignSingleton();
        }

        private void AssignSingleton()
        {
            if (singleton == null)
            {
                singleton = this;
                DontDestroyOnLoad(gameObject);
                return;
            }
            if(singleton != this) Destroy(this);
        }
        #endregion

        #region Saving
        private void Start()
        {
            if (singleton == this) LoadSettings();
        }

        public static void LoadSettings()
        {
            CheckSingleton();
            string path = singleton.UserSavePath;
#if UNITY_EDITOR
            path = singleton.EditorUserSavePath;
#endif
            if(singleton.SaveFilePreset != null) ConfigController.Repair($"{Application.persistentDataPath}/{path}", singleton.SaveFilePreset.text);
            if(!FileManager.TryLoadFileWriter($"{Application.persistentDataPath}/{path}", out _config)) return;
            List<string> sets = ConfigController.CreateOptionList(_config);

            for (int i = 0; i < sets.Count; i++)
            {
                string[] values = sets[i].Split(':');
                if (values.Length != 2) continue;
                if (!float.TryParse(values[1], out float result)) continue;
                ChangeParameterFloat(values[0], result, false);
            }
        }

        public static bool GetParameterFloat(string name, out float value)
        {
            CheckSingleton();
            value = 0f;
            if (singleton.Mixer == null) return false;
            return singleton.Mixer.GetFloat(name, out value);
        }

        public static void ChangeParameterFloat(string name, float value, bool preview = true)
        {
            CheckSingleton();
            if (singleton.Mixer == null || !singleton.Mixer.GetFloat(name, out _))
            {
                GameConsoleController.Log("Parameter or mixer does not exist! Cannot save or change parameter!", "error");
                return;
            }

            if (singleton.RoundValue) value = Mathf.Round(value);
            singleton.Mixer.SetFloat(name, value);

            if (!preview || string.IsNullOrWhiteSpace(singleton.UserSavePath)) return;

            string path = singleton.UserSavePath;
#if UNITY_EDITOR
            path = singleton.EditorUserSavePath;
#endif
            _config = ConfigController.SetSetting(_config, name, value.ToString());
            GameConsoleController.Log($"Changed parameter <b>{name}</b> to {value}", "settings");
            FileManager.SaveFileWriter($"{Application.persistentDataPath}/{path}", _config);
        }
        #endregion

        #region Channels
        private Dictionary<string, AudioChannel> channels = new Dictionary<string, AudioChannel>();

        private static AudioChannel GetChannel(string name)
        {
            if (singleton.channels.ContainsKey(name))
            {
                if(singleton.channels[name].source == null) singleton.channels[name].source = singleton.gameObject.AddComponent<AudioSource>();
                return singleton.channels[name];
            }

            AudioChannel data = new AudioChannel();
            data.source = singleton.gameObject.AddComponent<AudioSource>();
            return data;
        }

        private static void SetChannel(string name, AudioChannel channel)
        {
            if (singleton.channels.ContainsKey(name))
            {
                singleton.channels[name] = channel;
                return;
            }
            singleton.channels.Add(name, channel);
        }
        #endregion

        #region Play
        public static void Play(string channelName, AudioData data)
        {
            CheckSingleton();
            AudioChannel channel = GetChannel(channelName);
            if (!data.replace && channel.source.isPlaying) return;

            channel.source.clip = data.clip;
            channel.source.loop = data.loop;
            if (singleton.Mixer != null) channel.source.outputAudioMixerGroup = data.group;
            channel.source.Play();

            if (channel.destroyEnum != null)
            {
                singleton.StopCoroutine(channel.destroyEnum);
                channel.destroyEnum = null;
            }
            channel.destroyEnum = channel.DestroyOnPlaybackEnd();
            singleton.StartCoroutine(channel.destroyEnum);

            SetChannel(channelName, channel);
        }

        public static void Stop(string channelName)
        {
            CheckSingleton();
            if (!singleton.channels.ContainsKey(channelName)) return;
            AudioChannel channel = singleton.channels[channelName];

            if (channel.source == null) return;
            Destroy(channel.source);
            if(channel.destroyEnum != null) singleton.StopCoroutine(channel.destroyEnum);
            channel = new AudioChannel();

            SetChannel(channelName, channel);
        }

        public static void Pause(string channelName)
        {
            CheckSingleton();
            if (!singleton.channels.ContainsKey(channelName)) return;
            AudioChannel channel = singleton.channels[channelName];

            if (channel.source == null || !channel.source.isPlaying) return;
            channel.source.Pause();

            if (channel.destroyEnum != null)
            {
                singleton.StopCoroutine(channel.destroyEnum);
                channel.destroyEnum = null;
            }

            SetChannel(channelName, channel);
        }

        public static void UnPause(string channelName)
        {
            CheckSingleton();
            if (!singleton.channels.ContainsKey(channelName)) return;
            AudioChannel channel = singleton.channels[channelName];

            if (channel.source == null || channel.source.isPlaying) return;
            channel.source.UnPause();
            channel.destroyEnum = channel.DestroyOnPlaybackEnd();
            singleton.StartCoroutine(channel.destroyEnum);
            SetChannel(channelName, channel);
        }
        #endregion
    }
}