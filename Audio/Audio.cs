using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Dungeon
{
    public class Audio : MonoBehaviour
    {
        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(0.1f, 3f)] public float pitch = 1f;
            public bool loop = false;
            [HideInInspector] public AudioSource source;
        }

        [Header("音频混合组")] 
        [SerializeField] private AudioMixerGroup masterMixerGroup;

        [Header("音效层级设置")] [SerializeField]
        private List<AudioLayerSettings> audioLayers = new List<AudioLayerSettings>();

        [Header("音效列表")] 
        [SerializeField] private List<Sound> sounds = new List<Sound>();

        [Header("对象池设置")] 
        [SerializeField] private int initialPoolSize = 5;
        [SerializeField] private int maxPoolSize = 100;

        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
        private Dictionary<AudioLayer, AudioLayerSettings> layerSettingsDict;
        private Dictionary<AudioLayer, List<AudioSource>> activeSourcesByLayer;

        public static Audio Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Update()
        {
            // 定期清理已完成播放的音源
            foreach (var layer in activeSourcesByLayer.Keys)
            {
                activeSourcesByLayer[layer].RemoveAll(source => 
                    source == null || !source.isPlaying
                );
            }
        }

        private void Initialize()
        {
            // 初始化音频源对象池
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewAudioSource();
            }

            layerSettingsDict = new Dictionary<AudioLayer, AudioLayerSettings>();
            activeSourcesByLayer = new Dictionary<AudioLayer, List<AudioSource>>();

            // 初始化层级字典
            foreach (var layerSetting in audioLayers)
            {
                layerSettingsDict[layerSetting.layer] = layerSetting;
                activeSourcesByLayer[layerSetting.layer] = new List<AudioSource>();
            }

            // 建立音效名称到音效的映射
            foreach (var sound in sounds)
            {
                if (!soundDictionary.ContainsKey(sound.name))
                {
                    soundDictionary.Add(sound.name, sound);
                }
                else
                {
                    Debug.LogWarning($"重复的音效名称: {sound.name}");
                }
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            GameObject audioSourceGO = new GameObject("AudioSource_" + audioSourcePool.Count);
            audioSourceGO.transform.SetParent(transform);

            AudioSource newSource = audioSourceGO.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = masterMixerGroup;
            audioSourcePool.Enqueue(newSource);

            return newSource;
        }

        private AudioSource GetAvailableAudioSource()
        {
            // 从池中获取可用的音频源
            foreach (var source in audioSourcePool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // 如果池未满，创建新的音频源
            if (audioSourcePool.Count < maxPoolSize)
            {
                return CreateNewAudioSource();
            }

            // 池已满，返回null或复用最早的音频源
            Debug.LogWarning("音频源池已满，将复用最早的音频源");
            AudioSource oldestSource = audioSourcePool.Dequeue();
            audioSourcePool.Enqueue(oldestSource);
            oldestSource.Stop();
            return oldestSource;
        }

        public void PlayAudio(string audioName)
        {
            PlayAudio(audioName, AudioLayer.SFX, 1);
        }

        public void PlayAudio(string audioName, AudioLayer layer = AudioLayer.SFX, float volumeMultiplier = 1f)
        {
            if (soundDictionary.TryGetValue(audioName, out Sound sound))
            {
                if (!layerSettingsDict.TryGetValue(layer, out AudioLayerSettings layerSettings))
                {
                    Debug.LogWarning($"音效层级未配置: {layer}");
                    return;
                }

                if (layerSettings.mute) return;

                AudioSource source = GetAvailableAudioSource();
                if (source == null) return;

                ConfigureAudioSource(source, sound, layerSettings, volumeMultiplier);
                source.Play();

                // 记录活跃音源
                activeSourcesByLayer[layer].Add(source);
            }
            else
            {
                Debug.LogWarning($"音效未找到: {audioName}");
            }
        }

        private void ConfigureAudioSource(AudioSource source, Sound sound, AudioLayerSettings layerSettings,
            float volumeMultiplier)
        {
            source.clip = sound.clip;
            source.volume = sound.volume * layerSettings.defaultVolume * volumeMultiplier;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = layerSettings.mixerGroup;
        }

        public void StopAudio(string name)
        {
            foreach (var source in audioSourcePool)
            {
                if (source.isPlaying && source.clip != null &&
                    soundDictionary.TryGetValue(name, out Sound sound) &&
                    source.clip == sound.clip)
                {
                    source.Stop();
                }
            }
        }

        public void StopAllAudio()
        {
            foreach (var source in audioSourcePool)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }
        }

        public void SetMasterVolume(float volume)
        {
            if (masterMixerGroup != null)
            {
                // 将线性音量转换为对数音量
                float dbVolume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
                masterMixerGroup.audioMixer.SetFloat("MasterVolume", dbVolume);
            }
        }

        // 设置特定层级的音量
        public void SetLayerVolume(AudioLayer layer, float volume)
        {
            if (layerSettingsDict.TryGetValue(layer, out AudioLayerSettings settings))
            {
                settings.defaultVolume = Mathf.Clamp01(volume);
                UpdateActiveSourcesVolume(layer);

                // 如果是主音量，还需要设置Mixer
                if (layer == AudioLayer.Master && settings.mixerGroup != null)
                {
                    float dbVolume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
                    settings.mixerGroup.audioMixer.SetFloat("MasterVolume", dbVolume);
                }
            }
        }

        // 静音/取消静音特定层级
        public void MuteLayer(AudioLayer layer, bool mute)
        {
            if (layerSettingsDict.TryGetValue(layer, out AudioLayerSettings settings))
            {
                settings.mute = mute;
                foreach (var source in activeSourcesByLayer[layer])
                {
                    if (source != null)
                    {
                        source.mute = mute;
                    }
                }
            }
        }

        // 暂停/恢复特定层级
        public void PauseLayer(AudioLayer layer, bool pause)
        {
            if (activeSourcesByLayer.TryGetValue(layer, out List<AudioSource> sources))
            {
                foreach (var source in sources)
                {
                    if (source == null) continue;

                    if (pause) source.Pause();
                    else source.UnPause();
                }
            }
        }

        // 停止特定层级所有音效
        public void StopLayer(AudioLayer layer)
        {
            if (activeSourcesByLayer.TryGetValue(layer, out List<AudioSource> sources))
            {
                foreach (var source in sources)
                {
                    if (source != null && source.isPlaying)
                    {
                        source.Stop();
                    }
                }

                sources.Clear();
            }
        }

        // 更新活跃音源的音量
        private void UpdateActiveSourcesVolume(AudioLayer layer)
        {
            if (activeSourcesByLayer.TryGetValue(layer, out List<AudioSource> sources) &&
                layerSettingsDict.TryGetValue(layer, out AudioLayerSettings settings))
            {
                foreach (var source in sources)
                {
                    if (source != null && soundDictionary.TryGetValue(source.clip.name, out Sound sound))
                    {
                        source.volume = sound.volume * settings.defaultVolume;
                    }
                }
            }
        }
    }

    // 在Audio类中添加
    public enum AudioLayer
    {
        Master,
        BackgroundMusic,
        SFX,
        UI,
        Voice,
        Ambient
    }

    [System.Serializable]
    public class AudioLayerSettings
    {
        public AudioLayer layer;
        public AudioMixerGroup mixerGroup;
        [Range(0f, 1f)] public float defaultVolume = 1f;
        public bool mute = false;
    }
}
