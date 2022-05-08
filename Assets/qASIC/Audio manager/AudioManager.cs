using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using qASIC.FileManagement;
using qASIC.ProjectSettings;

namespace qASIC.AudioManagement
{
    [AddComponentMenu("")]
    public class AudioManager : MonoBehaviour
    {
        public static AudioMixer Mixer { get => AudioProjectSettings.Instance.mixer; }

        public static int ChannelCount { get => Singleton.channels.Count; }

        public static string Path { get; private set; }
        public static SerializationType SaveType { get; private set; }

        private static bool? _enabledOverride = null;
        public static bool Enabled => _enabledOverride ?? AudioProjectSettings.Instance.enableAudioManager;

        public static void OverrideEnabled(bool enabled)
        {
            if (enabled == _enabledOverride) return;
            _enabledOverride = enabled;

            if (!enabled)
            {
                DestroyManager();
            }
        }

        #region Initialization
        private static bool _initialized = false;
        public static bool Initialized => _initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (!Enabled || _initialized) return;
            _initialized = true;

            qDebug.Log($"Initializing Audio Manager v{qASIC.Internal.Info.AudioVersion}", "init");
            AudioProjectSettings settings = AudioProjectSettings.Instance;

            if (settings.createOnStart)
                GenerateManager();

#if UNITY_EDITOR
            CreateEditorLoader();
#else
            LoadSettings();
#endif
            qDebug.Log("Audio Manager initialization complete!", "audio");
        }

#if UNITY_EDITOR
        private static void CreateEditorLoader()
        {
            qDebug.Log($"[Editor] Creating audio manager loader", "init");
            DontDestroyOnLoad(new GameObject("[Temp] Audio Manager Loader", typeof(Internal.AudioManagerEditorLoader)));
        }
#endif
#endregion

        #region Singleton
        static AudioManager _instance;
        public static AudioManager Singleton 
        { 
            get
            {
                if (!Enabled)
                    throw new System.Exception("Audio Manager has been disabled!");

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

        public static bool ManagerExists =>
            _instance != null;

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

        internal static void DestroyManager()
        {
            if (!ManagerExists) return;
            Destroy(_instance);
            qDebug.Log("Destroyed Audio Manager", "audio");
        }

        private void Awake()
        {
            if (!Enabled) return;
            AssignSingleton();
        }

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
        internal static void LoadSettings()
        {
            if (!Enabled) return;
            AudioProjectSettings settings = AudioProjectSettings.Instance;

            switch (settings.serializationType)
            {
                case SerializationType.config:
                    LoadSettingsConfig(settings.savePath.ToString());
                    break;
                case SerializationType.none:
                    break;
                default:
                    qDebug.LogError($"Serialization type '{settings.serializationType}' is not supported by the Audio Manager!");
                    break;
            }
        }

        public static void LoadSettingsConfig(string path)
        {
            if (!Enabled) return;
            SaveType = SerializationType.config;
            Path = path;

            if (!ConfigController.TryCreateListFromFile(Path, out List<KeyValuePair<string, string>> list)) return;

            for (int i = 0; i < list.Count; i++)
            {
                if (!float.TryParse(list[i].Value, out float value)) continue;
                SetFloat(list[i].Key, value, true);
            }

            qDebug.Log("Audio Manager preferences successfully loaded!", "init");
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
            if (!Enabled) return;

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

            switch (SaveType)
            {
                case SerializationType.config:
                    ConfigController.SetSettingFromFile(settings.savePath.ToString(), name, value.ToString());
                    break;
            }
        }

        public static void SetVolume(string name, float value, bool preview = true) =>
            SetFloat(name, value == 0 ? -80f : Mathf.Log10(value) * 40f, preview);

        public static bool GetVolume(string name, out float value)
        {
            bool exists = GetFloat(name, out value);
            value = Mathf.Pow(10f, value / 40f);
            return exists;
        }
        #endregion

        #region Channels
        public Dictionary<string, AudioChannel> channels = new Dictionary<string, AudioChannel>();

        public static AudioChannel GetChannel(string name)
        {
            if (!Enabled) return null;
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
            if (!Enabled) return;
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
            if (!Enabled) return;
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
            if (!Enabled) return;
            if (!Singleton.channels.ContainsKey(channelName)) return;
            AudioChannel channel = Singleton.channels[channelName];

            if (channel.Source == null) return;
            Destroy(channel.Source);
            channel = new AudioChannel();

            SetChannel(channelName, channel);
        }

        public static void StopAll()
        {
            if (!Enabled) return;
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
            if (!Enabled) return;
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
            if (!Enabled) return;
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
            if (!Enabled) return;
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
            if (!Enabled) return;
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