using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Airhockey.Core {
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : PlayerBehaviour {
        [SerializeField, BoxGroup("Movement")] private float movementSpeed = 1f;
        [SerializeField, BoxGroup("Dash")] private float dashMaxVelocity = 1f;
        [SerializeField, BoxGroup("Dash")] private float dashChargeSpeed = 1f;
        [SerializeField, BoxGroup("Dash")] private float dashCooldown = 1f;

        private Rigidbody m_rigidbody;
        private Vector3 m_input;

        private bool m_dashCharging = false;
        private float m_dashChargeStrength = 0.0f;
        private bool m_dashOnCooldown = false;

        private void Awake() {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        public void OnMove(InputAction.CallbackContext ctx) {
            if (IsLocked) return;

            var i = ctx.ReadValue<Vector2>();
            m_input = new Vector3(i.x, 0f, i.y);
        }

        public void OnDash(InputAction.CallbackContext ctx) {
            if (IsLocked) return;

            if (!m_dashCharging && ctx.performed) {
                m_dashCharging = true;
                return;
            }

            if (!m_dashCharging || !ctx.canceled) return;

            m_dashCharging = false;
            m_rigidbody.AddForce(m_input * m_dashChargeStrength, ForceMode.Impulse);
            m_dashChargeStrength = 0.0f;
        }

        private void FixedUpdate() {
            Move();
        }

        private void Move() {
            if (IsLocked) {
                m_rigidbody.velocity = Vector3.zero;
                return;
            }

            if (m_dashCharging) {
                m_rigidbody.velocity = Vector3.zero;
                m_dashChargeStrength = Mathf.Clamp(m_dashChargeStrength + (dashChargeSpeed * Time.deltaTime), 0.0f,
                    dashMaxVelocity);
                return;
            }

            m_rigidbody.AddForce(m_input * movementSpeed, ForceMode.Acceleration);
        }

        private IEnumerator Cooldown(float duration) {
            m_dashOnCooldown = true;
            yield return new WaitForSeconds(duration);
            m_dashOnCooldown = false;
        }

        private void OnDrawGizmos() {
            if (m_rigidbody == null) return;
            var origin = transform.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, origin + m_input);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + m_rigidbody.velocity);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin, origin + m_input * m_dashChargeStrength);
        }
    }
}