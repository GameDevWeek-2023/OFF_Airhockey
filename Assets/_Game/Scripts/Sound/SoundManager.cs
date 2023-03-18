using System.Collections.Generic;
using Airhockey.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Airhockey.Sound {
    public class SoundManager : MonoSingleton<SoundManager> {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private List<SoundSet> sets;

        private GameObject m_SoundSourcesGameObject;
        private Dictionary<string, SoundSet> m_AudioSets;
        private readonly Dictionary<string, SoundLayer> m_Layers = new Dictionary<string, SoundLayer>();

        private GameObject SoundSourcesGameObject {
            get {
                if (m_SoundSourcesGameObject != null) return m_SoundSourcesGameObject;

                m_SoundSourcesGameObject = new GameObject("Audio Sources");
                return m_SoundSourcesGameObject;
            }
        }

        private Dictionary<string, SoundSet> AudioSets {
            get {
                if (m_AudioSets != null) return m_AudioSets;

                m_AudioSets = new Dictionary<string, SoundSet>();
                foreach (var audioSet in sets) {
                    m_AudioSets[audioSet.Key] = audioSet;
                }

                return m_AudioSets;
            }
        }

        public bool TryGetAudioSet(string setName, out SoundSet set) {
            return AudioSets.TryGetValue(setName, out set);
        }

        public SoundSet GetAudioSet(string setName) {
            return AudioSets[setName];
        }

        public AudioMixer Mixer => mixer;

        public bool SetGroupVolume(string @group, float value) {
            if (mixer == null) return true;

            return mixer.SetFloat(@group, ToDb(value));
        }

        public bool GetGroupVolume(string @group, out float value) {
            if (mixer == null) {
                value = 0.0f;
                return false;
            }

            mixer.GetFloat(@group, out value);
            value = ToLinear(value);
            return true;
        }

        private float ToDb(float value, float multiplier = 60.0f) {
            return Mathf.Log10(value + 0.01f) * multiplier + 20;
        }

        private float ToLinear(float value, float multiplier = 60.0f) {
            return Mathf.Pow(10, (value - 20) / multiplier) - 0.01f;
        }

        public void PlaySpatialSoundAt(AudioClip clip, Vector3 position, int priority = 1, float volume = 1.0f,
            float pitch = 1.0f, bool spatializePostEffects = false, float spread = 360.0f,
            bool ignoreListenerPause = false, string mixerGroup = "") {
            PlaySoundAt(clip, position, priority, volume, pitch, true, 1.0f, spatializePostEffects, spread,
                ignoreListenerPause, mixerGroup);
        }

        public void PlaySpatialSoundAt(SoundModule module, Vector3 position, int priority = 1,
            bool spatializePostEffects = false, float spread = 360.0f,
            bool ignoreListenerPause = false, string mixerGroup = "") {
            if (!module.IsValid) return;

            PlaySoundAt(module.Clip, position, priority, module.Volume, module.Pitch, true,
                1.0f, spatializePostEffects, spread, ignoreListenerPause, mixerGroup);
        }

        public void PlaySound(AudioClip clip, int priority = 1, float volume = 1.0f,
            float pitch = 1.0f,
            bool ignoreListenerPause = false, string mixerGroup = "") {
            PlaySoundAt(clip, Vector3.zero, priority, volume, pitch, false, 0.0f, false, 360.0f,
                ignoreListenerPause, mixerGroup);
        }

        public void PlaySound(SoundModule module, int priority = 1,
            bool ignoreListenerPause = false, string mixerGroup = "") {
            if (!module.IsValid) return;

            PlaySoundAt(module.Clip, Vector3.zero, priority, module.Volume, module.Pitch,
                false, 0.0f, false, 360.0f, ignoreListenerPause, mixerGroup);
        }

        private void PlaySoundAt(AudioClip clip, Vector3 position, int priority = 1, float volume = 1.0f,
            float pitch = 1.0f,
            bool spatialize = false,
            float spatialBlend = 0.0f, bool spatializePostEffects = false, float spread = 360.0f,
            bool ignoreListenerPause = false, string mixerGroup = "") {
            var obj = new GameObject("Sound Source") {
                transform = {
                    position = position
                }
            };

            var source = obj.AddComponent<AudioSource>();
            source.clip = clip;
            source.priority = priority;
            source.ignoreListenerPause = ignoreListenerPause;
            source.volume = volume;
            source.pitch = pitch;
            source.spatialize = spatialize;
            source.spatialBlend = spatialBlend;
            source.spatializePostEffects = spatializePostEffects;
            source.spread = spread;
            source.loop = false;

            var groups = Mixer.FindMatchingGroups(mixerGroup);
            if (groups.Length > 0) {
                source.outputAudioMixerGroup = groups[0];
            }

            source.Play();

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(clip.length);
            sequence.OnComplete(() => { Destroy(obj); });
        }


        public bool RequestLayer(string layerName, out SoundLayer layer) {
            if (!m_Layers.TryGetValue(layerName, out SoundLayer existingLayer)) {
                layer = new SoundLayer(layerName, SoundSourcesGameObject);
                m_Layers.Add(layerName, layer);
                return true;
            }

            if (!existingLayer.IsPlaying) {
                layer = existingLayer;
                return true;
            }

            Debug.LogWarning("[GlobalAudio]: layer is still in use returning null");
            layer = null;
            return false;
        }

        public bool ReleaseLayer(string layerName, bool forceRelease = false) {
            if (!m_Layers.TryGetValue(layerName, out SoundLayer existingLayer)) return false;

            if (existingLayer.IsPlaying && !forceRelease) return false;

            m_Layers.Remove(layerName);
            existingLayer.Destroy();
            return true;
        }

        public bool HasLayer(string layerName) {
            return m_Layers.ContainsKey(layerName);
        }
    }
}