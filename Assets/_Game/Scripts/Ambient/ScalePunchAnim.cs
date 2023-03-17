using DG.Tweening;
using UnityEngine;

namespace Airhockey.Ambient {
    public class ScalePunchAnim : Anim {
        [SerializeField] private Vector3 punch = Vector3.one;
        [SerializeField] private float duration = 0.1f;
        [SerializeField] private Ease ease;

        protected override Tween Animation() {
            return transform.DOPunchScale(punch, duration).SetEase(ease);
        }

        public void Animate() {
            Intern_Animate();
        }
    }
}