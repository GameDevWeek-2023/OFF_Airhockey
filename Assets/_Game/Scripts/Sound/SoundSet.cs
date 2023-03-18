using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Airhockey.Sound {
    [CreateAssetMenu(fileName = "new AudioSet", menuName = "Sound/Audio Set", order = 0)]
    public class SoundSet : ScriptableObject {
        [SerializeField] private new string key = "Audio Set";
        [SerializeField] private SoundModule[] sounds;
        public string Key => key;
        public bool IsValid => sounds.Length > 0;

        public SoundModule GetModule(int index) {
            return sounds[index];
        }

        public bool TryGetModule(string name, out SoundModule soundModule) {
            foreach (var module in sounds) {
                if (module.Name.Equals(name)) {
                    soundModule = module;
                    return true;
                }
            }

            soundModule = null;
            return false;
        }


        public bool TryGetRandomModule(out SoundModule soundModule) {
            if (!IsValid) {
                soundModule = null;
                return false;
            }

            var sum = 0.0f;
            foreach (var sound in sounds) {
                sum += sound.Weight;
            }

            var random = Random.Range(0f, sum);
            foreach (var sound in sounds) {
                random -= sound.Weight;
                if (random <= 0) {
                    soundModule = sound;
                    return true;
                }
            }

            soundModule = null;
            return false;
        }
    }

    [Serializable]
    public class SoundModule {
        [SerializeField] private string name = "Sound Module";
        [SerializeField] private float weight = 1.0f;
        [SerializeField] private AudioClip clip;

        [Space, SerializeField] private bool loop = false;
        [SerializeField, Range(0f, 1f)] private float volume = 1.0f;
        [SerializeField, Range(-3f, 3f)] private float pitch = 1.0f;

        public string Name => name;
        public float Weight => weight;
        public AudioClip Clip => clip;
        public bool Loop => loop;
        public float Volume => volume;
        public float Pitch => pitch;

        public bool IsValid => clip != null;
    }
}