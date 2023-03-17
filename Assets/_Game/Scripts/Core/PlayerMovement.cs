using System;
using System.Collections.Generic;
using System.Linq;
using Airhockey.Events;
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

        [SerializeField, BoxGroup("Dash"), Range(0f, 1f)]
        private float dashPredictionIntensity = 0.5f;

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

            if (ctx.performed) {
                StartCharging();
            }
            else if (ctx.canceled) {
                StopCharging();
            }
        }

        private void StartCharging() {
            if (DashCharging || DashOnCooldown) return;

            DashCharging = true;

            SlowmoManager.StartSlowmo(Player.Id);
        }

        private void Charge() {
            if (!DashCharging || DashOnCooldown) return;

            //m_rigidbody.velocity = Vector3.zero;
            m_dashCharge = Mathf.Clamp(m_dashCharge + (dashChargeSpeed * Time.fixedUnscaledDeltaTime), 0.0f,
                dashMaxVelocity);

            if (m_dashCharge >= dashMaxVelocity) {
                StopCharging();
            }
        }

        private void StopCharging() {
            if (!DashCharging || DashOnCooldown) return;

            m_aimDirection = AimDirection();

            m_rigidbody.AddForce(m_aimDirection * m_dashCharge, ForceMode.Impulse);
            m_dashCharge = 0.0f;

            DashCharging = false;
            DashOnCooldown = true;
            SlowmoManager.StopSlowmo(Player.Id);

            DOTween.Sequence().SetUpdate(true).AppendInterval(dashCooldown).OnComplete(() => DashOnCooldown = false);
        }

        private void FixedUpdate() {
            Charge();
            Move();
        }

        private void Move() {
            if (IsLocked || DashCharging) return;

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

            Gizmos.color = Color.white;

            /*var stepSize = dashAimConeAngle / dashAimIterations;

            var angle = Vector3.SignedAngle(Vector3.forward, Input, Vector3.up);
            for (int i = 0; i < dashAimIterations; i++) {
                var dir = DirFromAngle(angle, true);
                angle -= stepSize;

                Ray ray = new Ray(transform.position, dir);
                Gizmos.color = Physics.Raycast(ray, float.PositiveInfinity, dashAimMask)
                    ? Color.magenta
                    : Color.white;
                Gizmos.DrawLine(origin, origin + dir * 0.5f);
            }

            angle = Vector3.SignedAngle(Vector3.forward, Input, Vector3.up);
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
            Debug.Log("On Player lock");
            DashCharging = false;
            DashOnCooldown = false;

            SlowmoManager.StopSlowmo(Player.Id);

            m_dashCharge = 0.0f;
            m_rigidbody.velocity = Vector3.zero;
        }

        private Vector3 AimDirection() {
            var stepSize = dashAimConeAngle / dashAimIterations;
            var angle = Vector3.SignedAngle(Vector3.forward, Input, Vector3.up);
            for (int i = 0; i < dashAimIterations; i++) {
                var dir = DirFromAngle(angle, true);
                angle -= stepSize;

                Ray ray = new Ray(transform.position, dir);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, dashAimMask)) {
                    var result = dir;

                    if (hitInfo.collider.TryGetComponent(out Rigidbody rigidbody)) {
                        result += rigidbody.velocity * dashPredictionIntensity;
                    }

                    return result;
                }
            }

            angle = Vector3.SignedAngle(Vector3.forward, Input, Vector3.up);
            for (int i = 0; i < dashAimIterations; i++) {
                var dir = DirFromAngle(angle, true);
                angle += stepSize;

                Ray ray = new Ray(transform.position, dir);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, dashAimMask)) {
                    var result = dir;

                    if (hitInfo.collider.TryGetComponent(out Rigidbody rigidbody)) {
                        result += rigidbody.velocity * dashPredictionIntensity;
                    }

                    return result;
                }
            }

            return Input;
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
            if (!angleIsGlobal) {
                angleInDegrees += transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private void OnCollisionEnter(Collision collision) {
            Signals.Publish(AmbientSignal.OnTriggerScreenShake);
        }
    }
}