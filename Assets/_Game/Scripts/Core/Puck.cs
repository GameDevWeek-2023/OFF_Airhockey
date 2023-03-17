using UnityEngine;

namespace Airhockey.Core {
    [RequireComponent(typeof(Rigidbody))]
    public class Puck : MonoBehaviour {
        [SerializeField] private Vector2 maxVelocity;
        [SerializeField] private float durationToMax = 30f;

        private float m_elapsedTime = 0.0f;
        private float m_currentVelocityMax = 0.0f;
        private Rigidbody m_rigidbody;

        private void Awake() {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
            m_elapsedTime = Mathf.Clamp(m_elapsedTime + Time.fixedDeltaTime, 0f, durationToMax);
            var t = Mathf.Clamp01(m_elapsedTime / durationToMax);
            m_currentVelocityMax = Mathf.Lerp(maxVelocity.x, maxVelocity.y, t);
            m_rigidbody.velocity = Vector3.ClampMagnitude(m_rigidbody.velocity, m_currentVelocityMax);
        }

        public void Reset() {
            m_elapsedTime = 0.0f;
            m_rigidbody.velocity = Vector3.zero;
        }
    }
}