using System;
using UnityEngine;

namespace Airhockey.Ambient {
    [RequireComponent(typeof(Rigidbody))]
    public class VelocitySqueeze : MonoBehaviour {
        [SerializeField] private float maxVelocity = 10f;
        private Rigidbody m_rigidbody;

        private Vector3 m_originScale;

        private void Awake() {
            m_rigidbody = GetComponent<Rigidbody>();
            m_originScale = transform.localScale;
        }

        public void LateUpdate() {
            var velocity = m_rigidbody.velocity;
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);

            var x = Mathf.Clamp(velocity.magnitude, 0f, maxVelocity) / maxVelocity;
            var scale = transform.localScale;
            var diffX = m_originScale.x - scale.x;

            scale.x += diffX - x;
            transform.localScale = scale;
        }
    }
}