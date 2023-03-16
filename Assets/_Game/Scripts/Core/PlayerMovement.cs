using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
        [SerializeField, BoxGroup("Dash")] private float dashAimConeAngle = 45f;
        [SerializeField, BoxGroup("Dash")] private int dashAimIterations = 10;
        [SerializeField, BoxGroup("Dash")] private LayerMask dashAimMask;

        private Rigidbody m_rigidbody;
        private float m_dashCharge = 0.0f;
        private Vector3 m_aimDirection;
        public Vector3 Input { get; private set; }

        public bool DashCharging { get; private set; } = false;

        public float DashCharge => (m_dashCharge / dashMaxVelocity);
        public bool DashOnCooldown { get; private set; } = false;

        private void Awake() {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        public void OnMove(InputAction.CallbackContext ctx) {
            if (IsLocked) return;

            var i = ctx.ReadValue<Vector2>();
            Input = new Vector3(i.x, 0f, i.y);
        }

        public void OnDash(InputAction.CallbackContext ctx) {
            if (IsLocked) return;

            if (!DashCharging && !DashOnCooldown && ctx.performed) {
                DashCharging = true;
                DashOnCooldown = true;
                SlowmoManager.StartSlowmo(Player.Id);
                return;
            }

            if (!DashCharging || !ctx.canceled) return;

            m_aimDirection = AimDirection();

            m_rigidbody.AddForce(m_aimDirection * m_dashCharge, ForceMode.Impulse);
            m_dashCharge = 0.0f;

            DashCharging = false;
            SlowmoManager.StopSlowmo(Player.Id);
            DashOnCooldown = false;
            //DOTween.Sequence().AppendInterval(dashCooldown).OnComplete(() => DashOnCooldown = false);
        }

        private void FixedUpdate() {
            Move();
        }

        private void Move() {
            if (IsLocked) return;

            if (DashCharging) {
                m_rigidbody.velocity = Vector3.zero;
                m_dashCharge = Mathf.Clamp(m_dashCharge + (dashChargeSpeed * Time.fixedUnscaledDeltaTime), 0.0f,
                    dashMaxVelocity);
                return;
            }

            m_rigidbody.AddForce(Input * movementSpeed, ForceMode.Acceleration);
        }

        private void OnDrawGizmos() {
            if (m_rigidbody == null) return;
            var origin = transform.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, origin + Input);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + m_rigidbody.velocity);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin, origin + m_aimDirection * m_dashCharge);

            /*Gizmos.color = Color.white;
            var minAngle = -dashAimConeAngle;
            var maxAngle = dashAimConeAngle;
            var diff = minAngle - maxAngle;
            var stepSize = diff / dashAimIterations;

            var angle = Vector3.SignedAngle(Vector3.forward, Input, Vector3.up);
            angle -= minAngle;

            for (int i = 0; i < dashAimIterations; i++) {
                var dir = DirFromAngle(angle, true);
                angle += stepSize;

                Ray ray = new Ray(transform.position, dir);
                Gizmos.color = Physics.Raycast(ray, float.PositiveInfinity, dashAimMask)
                    ? Color.magenta
                    : Color.white;
                Gizmos.DrawLine(origin, origin + dir * 0.5f);
            }*/
        }

        public override void OnLock() {
            DashCharging = false;
            m_dashCharge = 0.0f;
            m_rigidbody.velocity = Vector3.zero;
        }

        private Vector3 AimDirection() {
            var minAngle = -dashAimConeAngle;
            var maxAngle = dashAimConeAngle;
            var diff = minAngle - maxAngle;
            var stepSize = diff / dashAimIterations;

            var angle = Vector3.SignedAngle(Vector3.forward, Input, Vector3.up);
            angle -= minAngle;

            List<Vector3> dirs = new List<Vector3>();
            RaycastHit hitInfo = new RaycastHit();
            for (int i = 0; i < dashAimIterations; i++) {
                var dir = DirFromAngle(angle, true);
                angle += stepSize;

                Ray ray = new Ray(transform.position, dir);
                if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, dashAimMask)) {
                    dirs.Add(dir);
                }
            }

            if (dirs.Count <= 0) return Input;

            var sum = Vector3.zero;
            foreach (var dir in dirs) {
                sum += dir;
            }

            var center = (sum / dirs.Count);

            if (hitInfo.collider != null && hitInfo.collider.TryGetComponent(out Rigidbody rigidbody)) {
                center += rigidbody.velocity * 0.5f;
            }

            return center;
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
            if (!angleIsGlobal) {
                angleInDegrees += transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}