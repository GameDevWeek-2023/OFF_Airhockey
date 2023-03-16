using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Airhockey.Ambient {
    [RequireComponent(typeof(PostProcessVolume))]
    public abstract class PostProcessAnim : MonoBehaviour {
        private PostProcessVolume m_postProcessVolume;
        protected PostProcessVolume Volume => m_postProcessVolume ??= GetComponent<PostProcessVolume>();

        public Sequence Animate(Tween animation) {
            var sequence = DOTween.Sequence();

            return sequence.Append(animation);
        }
    }
}