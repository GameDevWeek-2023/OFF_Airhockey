using Airhockey.Ambient;
using Airhockey.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts.Ambient {
    public class DrawbridgeAnim : Anim {
        [SerializeField] private int playerIndex = 0;
        [SerializeField] private float targetAngle = 0f;
        [SerializeField] private float duration = 3.5f;
        [SerializeField] private Ease ease = Ease.InBounce;
        [SerializeField] private UnityEvent onOpened;

        protected override Tween Animation() {
            var eulerAngles = transform.localRotation.eulerAngles;
            eulerAngles.x = targetAngle;
            return transform.DOLocalRotate(eulerAngles, duration, RotateMode.Fast).SetEase(ease);
        }

        private void OnEnable() {
            Signals.Subscribe(PlayerSignal.OnPlayerJoined, OnPlayerJoined);
        }

        private void OnDisable() {
            Signals.Unsubscribe(PlayerSignal.OnPlayerJoined, OnPlayerJoined);
        }

        private void OnPlayerJoined(Signals.Args obj) {
            if (!obj.Read(out int index) || index != playerIndex) return;
            
            onOpened?.Invoke();
            Intern_Animate();
        }
    }
}