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
        }
    }
}