using System;
using UnityEngine;

namespace Airhockey.Utils {
    [RequireComponent(typeof(Rigidbody))]
    public class ClampVelocity : MonoBehaviour {
        [SerializeField] private float maxVelocity = 5f;
        private Rigidbody m_rigidbody;

        private void Awake() {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, maxVelocity);
        }
    }
}