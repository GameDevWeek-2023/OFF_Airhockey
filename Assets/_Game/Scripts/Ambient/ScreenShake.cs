using Airhockey.Events;
using DG.Tweening;
using UnityEngine;

namespace Airhockey.Ambient {
    public class ScreenShake : Anim {
        [SerializeField] private Vector3 punch = Vector3.one;
        [SerializeField] private float duration = 0.1f;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float elasticity = 1.0f;


        private void OnEnable() {
            Signals.Subscribe(AmbientSignal.OnTriggerScreenShake, OnTrigger);
        }

        private void OnDisable() {
            Signals.Unsubscribe(AmbientSignal.OnTriggerScreenShake, OnTrigger);
        }

        private void OnTrigger(Signals.Args obj) {
            Intern_Animate();
        }

        protected override Tween Animation() {
            return transform.DOPunchRotation(punch, duration, vibrato, elasticity);
        }
    }
}