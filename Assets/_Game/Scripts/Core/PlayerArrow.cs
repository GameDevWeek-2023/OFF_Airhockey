using System;
using UnityEngine;

namespace Airhockey.Core {
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerArrow : PlayerBehaviour {
        [SerializeField] private MeshRenderer inputArrow;
        [SerializeField] private MeshRenderer chargeArrow;

        private PlayerMovement m_movement;
        private MaterialPropertyBlock m_inputPropertyBlock;
        private MaterialPropertyBlock m_chargePropertyBlock;

        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int ProgressId = Shader.PropertyToID("_Progress");


        private void Awake() {
            m_movement = GetComponent<PlayerMovement>();

            m_inputPropertyBlock = new MaterialPropertyBlock();
            m_chargePropertyBlock = new MaterialPropertyBlock();
        }

        private void Start() {
            if (!Player.TryGetDetails(out PlayerDetails details)) return;

            m_chargePropertyBlock.SetColor(ColorId, details.color);

            SetColor(inputArrow, m_inputPropertyBlock, details.color);
            SetColor(chargeArrow, m_chargePropertyBlock, details.color);
        }

        private void SetColor(MeshRenderer renderer, MaterialPropertyBlock block, Color color) {
            renderer.GetPropertyBlock(block);
            block.SetColor(ColorId, color);
            renderer.SetPropertyBlock(block);
        }

        private void LateUpdate() {
            var aim = m_movement.Input != Vector3.zero ? m_movement.Input : transform.forward;
            inputArrow.transform.rotation =
                Quaternion.Euler(0f, -180f, 0.0f) * Quaternion.LookRotation(aim, Vector3.up);


            chargeArrow.enabled = m_movement.DashCharging;
            chargeArrow.transform.rotation =
                Quaternion.Euler(0f, -180f, 0.0f) * Quaternion.LookRotation(aim, Vector3.up);

            chargeArrow.GetPropertyBlock(m_chargePropertyBlock);
            m_chargePropertyBlock.SetFloat(ProgressId, m_movement.DashCharge);
            chargeArrow.SetPropertyBlock(m_chargePropertyBlock);
        }

        /*[SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float maxDistance = 1f;

        private PlayerMovement m_movement;
        private Vector3 m_input;

        private MaterialPropertyBlock m_propertyBlock;
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int ProgressId = Shader.PropertyToID("_Progress");

        private void Awake() {
            m_movement = GetComponent<PlayerMovement>();
        }

        private void Start() {
            m_propertyBlock = new MaterialPropertyBlock();

            if (!Player.TryGetDetails(out PlayerDetails details)) return;

            m_propertyBlock.SetColor(ColorId, details.color);
            lineRenderer.SetPropertyBlock(m_propertyBlock);
        }

        private void LateUpdate() {
            var dist = m_movement.DashCharging ? maxDistance : 0.0f;
            var start = lineRenderer.transform.position + m_movement.Input * 0.15f;
            var end = start + m_movement.Input * dist;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            m_propertyBlock.SetFloat(ProgressId, m_movement.DashCharge);
            lineRenderer.SetPropertyBlock(m_propertyBlock);
        }*/
    }
}