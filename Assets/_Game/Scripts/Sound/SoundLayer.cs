using UnityEngine;

namespace Airhockey.Sound {
    public class SoundLayer {
        public GameObject Parent { get; }
        public AudioSource Source { get; }
        public string Name { get; }

        public bool IsPlaying => Source.isPlaying;
            
        public SoundLayer(string name, GameObject parent) {
            Name = name;
            Parent = parent;
            Source = Parent.AddComponent<AudioSource>();
            Source.playOnAwake = false;
            Source.spatialize = false;
            Source.spatialBlend = 0.0f;
        }

        public void Destroy() {
            Object.Destroy(this.Source);
        }
    }
}