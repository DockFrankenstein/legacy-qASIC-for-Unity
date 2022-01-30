using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using qASIC.FileManagement;
using qASIC.ProjectSettings;

namespace qASIC.AudioManagment
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioMixer Mixer { get => AudioProjectSettings.Instance.mixer; }

        public static int ChannelCount { get => Singleton.channels.Count; }

        #region Initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            AudioProjectSettings settings = AudioProjectSettings.Instance;
            LoadSettings();

            if (settings.createOnStart)
                GenerateManager();
        }
        #endregion

        #region Singleton
        static AudioManager _instance;
        public static AudioManager Singleton 
        { 
            get
            {
                if (_instance == null)
                {
                    if (!AudioProjectSettings.Instance.createOnUse)
                        throw new UnassignedReferenceException("Audio Manager create on use has been disabled!");

                    GenerateManager();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        internal static bool GenerateManager()
        {
            AudioProjectSettings settings = AudioProjectSettings.Instance;

            try
            {
                AudioManager manager = new GameObject(settings.managerName).AddComponent<AudioManager>();
                manager.AssignSingleton();
            }
            catch (System.Exception e)
            {
                if (settings.logCreationError)
                    qDebug.Log($"{settings.creationErrorMessage}{e}", settings.creationErrorColor);

                return false;
            }

            if (settings.logCreation)
                qDebug.Log(settings.creationLogMessage, settings.creationLogColor);

            return true;
        }

        private void Awake() => AssignSingleton();

        public static bool Paused = false;

        private void AssignSingleton()
        {
            if (_instance == null)
            {
                Singleton = this;
                DontDestroyOnLoad(gameObject);
                return;
            }

            if (_instance != this)
                Destroy(gameObject);
        }
        #endregion

        #region Saving
        public static void LoadSettings()
        {
            LoadSettingsConfig();
        }

        public static void LoadSettingsConfig()
        {
            AudioProjectSettings settings = AudioProjectSettings.Instance;
            if (!ConfigController.TryCreateListFromFile(settings.SavePath, out List<KeyValuePair<string, string>> list)) return;

            for (int i = 0; i < list.Count; i++)
            {
                if (!float.TryParse(list[i].Value, out float value)) continue;
                SetFloat(list[i].Key, value, true);
            }
        }
        #endregion

        #region Parameters
        public static bool GetFloat(string name, out float value)
        {
            value = 0f;
            if (Mixer == null) return false;
            return Mixer.GetFloat(name, out value);
        }

        public static void SetFloat(string name, float value, bool preview = true)
        {
            AudioProjectSettings settings = AudioProjectSettings.Instance;

            if (Mixer == null || !Mixer.GetFloat(name, out _))
            {
                if (!preview)
                    qDebug.LogError("Parameter or mixer does not exist! Cannot save or change parameter!");
                return;
            }

            if (settings.roundValues)
                value = Mathf.Round(value);

            Mixer.SetFloat(name, value);

            if (preview) return;
            qDebug.Log($"Changed parameter '{name}' to '{value}'", "settings");
            ConfigController.SetSettingFromFile(settings.SavePath, name, value.ToString());
        }
        #endregion

        #region Channels
        public Dictionary<string, AudioChannel> channels = new Dictionary<string, AudioChannel>();

        public static AudioChannel GetChannel(string name)
        {
            if (Singleton.channels.ContainsKey(name))
            {
                if(Singleton.channels[name].Source == null)
                    Singleton.channels[name].Source = Singleton.gameObject.AddComponent<AudioSource>();
                return Singleton.channels[name];
            }

            AudioChannel data = new AudioChannel();
            data.Source = Singleton.gameObject.AddComponent<AudioSource>();
            return data;
        }

        public static void SetChannel(string name, AudioChannel channel)
        {
            if (Singleton.channels.ContainsKey(name))
            {
                Singleton.channels[name] = channel;
                return;
            }
            Singleton.channels.Add(name, channel);
        }

        static void StartDestroyCoroutine(ref AudioChannel channel)
        {
            if (channel.DestroyEnum != null)
            {
                Singleton.StopCoroutine(channel.DestroyEnum);
                channel.DestroyEnum = null;
            }
            channel.DestroyEnum = channel.DestroyOnPlaybackEnd();
            Singleton.StartCoroutine(channel.DestroyEnum);
        }

        static void StopDestroyCoroutine(ref AudioChannel channel)
        {
            if (channel.DestroyEnum != null)
            {
                Singleton.StopCoroutine(channel.DestroyEnum);
                channel.DestroyEnum = null;
            }
        }
        #endregion

        #region Play
        public static void Play(string channelName, AudioData data)
        {
            AudioChannel channel = GetChannel(channelName);
            if (!data.replace && channel.Source.isPlaying) return;

            channel.Source.clip = data.clip;
            channel.Source.loop = data.loop;
            channel.useGlobalControlls = data.useGlobalControls;

            if (Mixer != null) channel.Source.outputAudioMixerGroup = data.group;

            channel.Source.Play();
            if (!Paused || !channel.useGlobalControlls) StartDestroyCoroutine(ref channel);
            else channel.Source.Pause();

            SetChannel(channelName, channel);
        }

        public static void Stop(string channelName)
        {
            if (!Singleton.channels.ContainsKey(channelName)) return;
            AudioChannel channel = Singleton.channels[channelName];

            if (channel.Source == null) return;
            Destroy(channel.Source);
            channel = new AudioChannel();

            SetChannel(channelName, channel);
        }

        public static void StopAll()
        {
            AudioSourceController.OnStopAll.Invoke();

            Dictionary<string, AudioChannel> temp = new Dictionary<string, AudioChannel>(Singleton.channels);
            foreach (var item in temp)
            {
                if (Singleton.channels[item.Key].Source == null || !Singleton.channels[item.Key].useGlobalControlls) continue;
                Destroy(Singleton.channels[item.Key].Source);
                SetChannel(item.Key, new AudioChannel());
            }
        }

        public static void Pause(string channelName)
        {
            if (!Singleton.channels.ContainsKey(channelName)) return;
            AudioChannel channel = Singleton.channels[channelName];

            if (channel.Source == null || !channel.Source.isPlaying) return;
            channel.Source.Pause();
            channel.paused = true;

            StopDestroyCoroutine(ref channel);

            SetChannel(channelName, channel);
        }

        public static void PauseAll()
        {
            AudioSourceController.OnPauseAll.Invoke();

            Paused = true;
            Dictionary<string, AudioChannel> temp = new Dictionary<string, AudioChannel>(Singleton.channels);
            foreach (var item in temp)
            {
                AudioChannel channel = Singleton.channels[item.Key];
                if (channel.Source == null || !channel.Source.isPlaying || !channel.useGlobalControlls) continue;
                channel.Source.Pause();

                StopDestroyCoroutine(ref channel);

                SetChannel(item.Key, channel);
            }
        }

        public static void UnPause(string channelName)
        {
            if (!Singleton.channels.ContainsKey(channelName)) return;
            AudioChannel channel = Singleton.channels[channelName];

            if (channel.Source == null || channel.Source.isPlaying) return;
            channel.Source.UnPause();
            channel.paused = false;

            StartDestroyCoroutine(ref channel);

            SetChannel(channelName, channel);
        }

        public static void UnPauseAll()
        {
            AudioSourceController.OnUnPauseAll.Invoke();

            Paused = false;
            Dictionary<string, AudioChannel> temp = new Dictionary<string, AudioChannel>(Singleton.channels);
            foreach (var item in temp)
            {
                AudioChannel channel = Singleton.channels[item.Key];
                if (channel.Source == null || channel.Source.isPlaying || channel.paused || !channel.useGlobalControlls) continue;
                channel.Source.UnPause();

                StartDestroyCoroutine(ref channel);

                SetChannel(item.Key, channel);
            }
        }
        #endregion
    }
}