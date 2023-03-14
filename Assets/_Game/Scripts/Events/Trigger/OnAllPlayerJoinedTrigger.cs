using UnityEngine;
using UnityEngine.InputSystem;

namespace Airhockey.Events.Trigger {
    [RequireComponent(typeof(PlayerInputManager))]
    public class OnAllPlayerJoinedTrigger : MonoBehaviour {
        [SerializeField] private Signal signal;
        private PlayerInputManager m_playerInputManager;

        private void Awake() {
            m_playerInputManager = GetComponent<PlayerInputManager>();
        }

        private void OnEnable() {
            m_playerInputManager.onPlayerJoined += OnPlayerJoined;
        }

        private void OnDisable() {
            m_playerInputManager.onPlayerJoined -= OnPlayerJoined;
        }

        private void OnPlayerJoined(PlayerInput playerInput) {
            Debug.Log("Player Joint");
            if (m_playerInputManager.playerCount < m_playerInputManager.maxPlayerCount) return;
            
            if(signal != null)
                signal.Publish();
        }
    }
}