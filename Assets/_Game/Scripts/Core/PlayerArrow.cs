using System;
using UnityEngine;

namespace Airhockey.Core {
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerArrow : PlayerBehaviour {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float maxDistance = 1f;

        private PlayerMovement m_movement;
        private Vector3 m_input;

        private MaterialPropertyBlock m_propertyBlock;
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private void Awake() {
            m_movement = GetComponent<PlayerMovement>();
        }


        private void Start() {
            var color = Player.Details.color;

            m_propertyBlock = new MaterialPropertyBlock();
            m_propertyBlock.SetColor(ColorId, color);

            lineRenderer.SetPropertyBlock(m_propertyBlock);
        }

        private void LateUpdate() {
            var start = transform.position + m_movement.Input * 0.15f;
            var end = start + m_movement.Input * (m_movement.DashCharge * maxDistance);

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
    }
}