using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Airhockey.Interaction {
    [RequireComponent(typeof(Camera))]
    public class RayInteractor : MonoBehaviour {
        [SerializeField] private InputAction clickAction;
        [SerializeField] private LayerMask mask;

        private Camera m_camera;
        private IInteract m_selected;
        private Vector3 m_cursorPosition;

        private void Awake() {
            m_camera = GetComponent<Camera>();
        }

        private void OnEnable() {
            clickAction.Enable();
            clickAction.performed += OnInteract;
        }

        private void OnDisable() {
            clickAction.Disable();
            clickAction.performed -= OnInteract;
        }

        private void OnInteract(InputAction.CallbackContext obj) {
            m_selected?.OnInteract(-1);
        }

        private void Update() {
            var position = Mouse.current.position.ReadValue();
            m_cursorPosition = new Vector3(position.x, position.y, 0.0f);

            UpdateSelection();
        }


        private void UpdateSelection() {
            var ray = m_camera.ScreenPointToRay(m_cursorPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, mask)) {
                m_selected = null;
                return;
            }

            if (!hit.collider.TryGetComponent(out IInteract interact)) {
                m_selected?.OnHoverExit();
                m_selected = null;
                return;
            }

            if (interact != m_selected) {
                interact.OnHoverEnter();
            }

            m_selected = interact;
        }
    }
}