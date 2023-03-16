using System.Collections.Generic;
using System.Linq;
using Airhockey.Events;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Airhockey.Core {
    [RequireComponent(typeof(PlayerInputManager))]
    public class Arena : MonoBehaviour {
        [SerializeField, Required] private GameObject puckPrefab;

        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private Transform[] puckSpawns;

        private readonly Dictionary<int, Player> m_players = new Dictionary<int, Player>();
        private PlayerInputManager m_playerInputManager;
        private int m_maxPlayerCount = 2;
        private GameObject m_puckObject;

        public int PlayerCount => m_players.Count;
        public Player[] Players => m_players.Values.ToArray();

        private void Awake() {
            m_playerInputManager = GetComponent<PlayerInputManager>();
            m_maxPlayerCount = m_playerInputManager.maxPlayerCount;
        }

        public void OnPlayerJoined(PlayerInput playerInput) {
            var index = playerInput.playerIndex;
            if (!playerInput.TryGetComponent(out Player player)) return;

            player.Join(index);
            player.transform.SetParent(transform);
            m_players.Add(index, player);

            var spawn = playerSpawns[index];
            player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);

            Signals.Publish(PlayerSignal.OnPlayerJoined, index);
            if (index < m_maxPlayerCount - 1) return;

            Signals.Publish(PlayerSignal.OnAllPlayerJoined);

            SpawnPuck(Random.Range(0, m_maxPlayerCount));

            if (TryGetComponent(out Match match)) {
                match.Begin();
            }
        }

        public void SpawnPuck(int index) {
            if (m_puckObject == null) {
                m_puckObject = Instantiate(puckPrefab);
            }

            var spawn = puckSpawns[index];
            m_puckObject.transform.SetPositionAndRotation(spawn.position, spawn.rotation);

            if (m_puckObject.TryGetComponent(out Rigidbody rigidbody)) {
                rigidbody.velocity = Vector3.zero;
            }
        }

        public bool LockPlayer {
            set {
                foreach (var pair in m_players) {
                    pair.Value.IsLocked = value;
                }
            }
        }

        public void ResetPlayer() {
            foreach (var pair in m_players) {
                var spawn = playerSpawns[pair.Key];
                pair.Value.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            }
        }
    }
}