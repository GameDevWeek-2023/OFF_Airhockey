using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts.Trigger {
    public class OnCollisionTrigger : MonoBehaviour {
        [Serializable]
        public class CollisionEvent : UnityEvent<Collision> { }

        [SerializeField] private CollisionEvent onCollisionEnter, onCollisionExit, onCollisionStay;

        private void OnCollisionEnter(Collision other) {
            onCollisionEnter?.Invoke(other);
        }

        private void OnCollisionExit(Collision other) {
            onCollisionExit?.Invoke(other);
        }

        private void OnCollisionStay(Collision other) {
            onCollisionStay?.Invoke(other);
        }
    }
}