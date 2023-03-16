using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Ambient {
    public class ScalePunchAnim : Anim {
        [SerializeField] private Vector3 punch = Vector3.one;
        [SerializeField] private float duration = 0.1f;

        protected override Tween Animation() {
            return transform.DOPunchScale(punch, duration);
        }

        public void Animate() {
            Intern_Animate();
        }
    }
}