using Airhockey.Utils;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Airhockey.Core {
    public class Portal : MonoBehaviour {
        [SerializeField] private LayerMask mask;
        [SerializeField] private float cooldown = 2f;
        [SerializeField] private MeshRenderer renderer;
        [Foldout("Events")] public UnityEvent onSpawned, onDespawned, onEnter, onExit;
        private PortalManager m_manager;
        private int m_position;

        private bool m_isOpen = false;
        private bool m_isBlocked = false;

        public void Init(PortalManager manager, int position) {
            m_manager = manager;
            m_position = position;
        }

        public void OnSpawned() {
            m_isOpen = true;
            renderer.material.DOFloat(1.0f, "_Cutoff", 1.0f);
            onSpawned?.Invoke();
        }

        public void OnDespawned() {
            m_isOpen = false;
            renderer.material.DOFloat(0.0f, "_Cutoff", 1.0f).OnComplete(() => Destroy(this.gameObject));
            onDespawned?.Invoke();
        }

        private void OnTriggerEnter(Collider other) {
            if (!m_isOpen || m_isBlocked) return;

            if (!other.gameObject.layer.IsInLayerMask(mask)) return;

            onEnter?.Invoke();
            DOTween.Sequence().AppendCallback(() => { m_manager.GetNextPortal(m_position).Transport(other.transform); })
                .AppendInterval(cooldown);
        }

        public void Transport(Transform target) {
            DOTween.Sequence().AppendCallback(() => {
                m_isBlocked = true;
                target.transform.SetPositionAndRotation(transform.position,
                    transform.rotation);
                onExit?.Invoke();
            }).AppendInterval(cooldown).OnComplete(() => m_isBlocked = false);
        }
    }
}