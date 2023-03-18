using UnityEngine;

namespace Airhockey.Sound {
    public class SoundPlayer : MonoBehaviour {
        [SerializeField] private string setName;
        [SerializeField] private int priority = 1;
        [SerializeField] private string mixerGroup;
        [SerializeField] private bool spatial = false;
        [SerializeField] private bool playOnAwake = false;

        private SoundManager SoundManager => SoundManager.Instance;

        private void Start() {
            if (playOnAwake) {
                Play();
            }
        }

        public void Play() {
            if (!SoundManager.TryGetAudioSet(setName, out SoundSet set) ||
                !set.TryGetRandomModule(out SoundModule module)) {
                Debug.LogWarning($"[SoundPlayer]: could not find AudioSet {setName}!");
                return;
            }

            if (spatial) {
                SoundManager.PlaySpatialSoundAt(module, transform.position, priority, mixerGroup: mixerGroup);
                return;
            }

            SoundManager.PlaySound(module, priority, mixerGroup: mixerGroup);
        }
    }
}