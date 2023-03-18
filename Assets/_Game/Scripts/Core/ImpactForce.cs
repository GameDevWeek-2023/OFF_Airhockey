using UnityEngine;

namespace Airhockey.Core {
    [RequireComponent(typeof(Rigidbody))]
    public class ImpactForce : MonoBehaviour {
        [SerializeField] private float strength = 1f;

        private Rigidbody m_rigidbody;

        private void Awake() {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        public void Apply() {
            m_rigidbody.AddForce(transform.right * strength, ForceMode.Impulse);
        }
    }
}