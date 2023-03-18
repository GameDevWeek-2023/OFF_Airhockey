using System;
using System.Collections.Generic;
using System.Linq;
using Airhockey.Events;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Airhockey.Core {
    [RequireComponent(typeof(PlayerInputManager))]
    public class Arena : MonoBehaviour {
        [SerializeField, Required] private GameObject puckPrefab;

        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private Transform[] puckSpawns;

        [SerializeField] private float resetDuration = 3f;

        private readonly Dictionary<int, Player> m_players = new Dictionary<int, Player>();
        private PlayerInputManager m_playerInputManager;
        private int m_maxPlayerCount = 2;
        private Puck m_puck;

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
            if (m_puck == null) {
                var obj = Instantiate(puckPrefab);
                if (!obj.TryGetComponent(out Puck puck)) return;

                m_puck = puck;
            }

            var spawn = puckSpawns[index];

            m_puck.Reset();
            m_puck.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
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

        public void ResetArena(int puckSpawnIndex, TweenCallback callback) {
            var sequence = DOTween.Sequence();

            foreach (var pair in m_players) {
                var playerSpawn = playerSpawns[pair.Key];
                sequence.Insert(0, pair.Value.transform.DOMove(playerSpawn.position, resetDuration));
                sequence.Insert(0, pair.Value.transform.DORotateQuaternion(playerSpawn.rotation, resetDuration));
            }

            var puckSpawn = puckSpawns[puckSpawnIndex];
            m_puck.Reset();
            sequence.Insert(0, m_puck.transform.DOMove(puckSpawn.position, resetDuration));
            sequence.Insert(0, m_puck.transform.DORotateQuaternion(puckSpawn.rotation, resetDuration));

            sequence.OnComplete(callback);
            sequence.SetEase(Ease.OutBounce);
        }
    }
}