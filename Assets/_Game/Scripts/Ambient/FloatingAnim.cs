using System;
using NaughtyAttributes;
using UnityEngine;

namespace Airhockey.Ambient {
    public class FloatingAnim : MonoBehaviour {
        [SerializeField] private float timeOffset = 0.0f;
        [SerializeField, BoxGroup("Hover")] private float hoverStrength = 1f, hoverSpeed = 1f;
        [SerializeField, BoxGroup("Rotation")] private float rotationStrength = 3f, rotationSpeed = 1f;

        private Quaternion m_startRotation;
        private Vector3 m_startPosition;
        private float m_elapsedTime;

        private void Awake() {
            m_startPosition = transform.position;
            m_startRotation = transform.rotation;

            m_elapsedTime = timeOffset;
        }

        private void LateUpdate() {
            m_elapsedTime += Time.deltaTime;

            var euler = new Vector3(Mathf.Sin(m_elapsedTime * rotationSpeed), Mathf.Cos(m_elapsedTime * rotationSpeed),
                0.0f);
            euler *= rotationStrength;

            transform.rotation = m_startRotation * Quaternion.Euler(euler);
            transform.position = m_startPosition + Vector3.up * (Mathf.Sin(m_elapsedTime * hoverSpeed) * hoverStrength);
        }
    }
}