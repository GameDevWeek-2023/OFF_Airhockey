using DG.Tweening;
using UnityEngine;

namespace Airhockey.Ambient {
    public abstract class Anim : MonoBehaviour {
        [SerializeField] protected float cooldown = 0.1f;
        private bool m_isAnimating;

        protected abstract Tween Animation();

        protected bool Intern_Animate() {
            if (m_isAnimating) return false;

            m_isAnimating = true;
            DOTween.Sequence().Append(Animation()).AppendInterval(cooldown)
                .AppendCallback(() => m_isAnimating = false);
            return true;
        }
    }
}