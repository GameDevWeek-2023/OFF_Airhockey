using System;
using Airhockey.Utils;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts.Trigger {
    public class OnCollisionTrigger : MonoBehaviour {
        [SerializeField] private bool useCooldown = false;

        [SerializeField, EnableIf("useCooldown")]
        private float cooldown = 1f;

        [SerializeField] private LayerMask mask;

        [Serializable]
        public class CollisionEvent : UnityEvent<Collision> { }

        [SerializeField] private CollisionEvent onCollisionEnter, onCollisionExit, onCollisionStay;
        private bool m_onCollisionEnterOnCooldown = false;
        private bool m_onCollisionExitOnCooldown = false;
        private bool m_onCollisionStayOnCooldown = false;

        private void OnCollisionEnter(Collision other) {
            if (!other.gameObject.layer.IsInLayerMask(mask) || m_onCollisionEnterOnCooldown) return;

            m_onCollisionEnterOnCooldown = true;
            onCollisionEnter?.Invoke(other);
            StartCooldown(() => m_onCollisionEnterOnCooldown = false);
        }

        private void OnCollisionExit(Collision other) {
            if (!other.gameObject.layer.IsInLayerMask(mask) || m_onCollisionExitOnCooldown) return;

            m_onCollisionExitOnCooldown = true;
            onCollisionExit?.Invoke(other);
            StartCooldown(() => m_onCollisionExitOnCooldown = false);
        }

        private void OnCollisionStay(Collision other) {
            if (!other.gameObject.layer.IsInLayerMask(mask) || m_onCollisionStayOnCooldown) return;

            m_onCollisionStayOnCooldown = true;
            onCollisionStay?.Invoke(other);
            StartCooldown(() => m_onCollisionStayOnCooldown = false);
        }

        private void StartCooldown(TweenCallback onComplete) {
            DOTween.Sequence().AppendInterval(cooldown).OnComplete(onComplete);
        }
    }
}