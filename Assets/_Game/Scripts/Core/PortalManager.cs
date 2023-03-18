using System;
using System.Collections.Generic;
using System.Linq;
using Airhockey.Events;
using Airhockey.Utils;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Airhockey.Core {
    public class PortalManager : MonoSingleton<PortalManager> {
        [SerializeField] private Transform[] spawns;
        [SerializeField] private GameObject prefab;

        [SerializeField] private int maxPortalSpawn;
        [SerializeField] private float waitForSpawn = 10f;
        [SerializeField] private float waitForDespawn = 10f;

        private List<int> m_freeSpawns = new List<int>();
        private List<int> m_occupiedSpawns = new List<int>();
        private Dictionary<int, Portal> m_portals = new Dictionary<int, Portal>();

        private bool m_isRunning = false;
        private Sequence m_sequence;

        private void Awake() {
            for (var i = 0; i < spawns.Length; i++) {
                m_freeSpawns.Add(i);
            }
        }

        public Portal GetNextPortal(int spawnIndex) {
            if (!m_portals.ContainsKey(spawnIndex)) return null;

            var keys = m_portals.Keys.ToList();
            keys.Remove(spawnIndex);

            var elem = Random.Range(0, keys.Count);
            return m_portals[keys[elem]];
        }

        private void OnEnable() {
            Signals.Subscribe(GameSignal.OnStartSpawnPortals, OnStartSpawnPortals);
            Signals.Subscribe(GameSignal.OnStopSpawnPortals, OnStopSpawnPortals);
        }

        private void OnDisable() {
            Signals.Unsubscribe(GameSignal.OnStartSpawnPortals, OnStartSpawnPortals);
            Signals.Unsubscribe(GameSignal.OnStopSpawnPortals, OnStopSpawnPortals);
        }

        private void OnStartSpawnPortals(Signals.Args obj) {
            m_isRunning = true;

            DespawnPortals();
        }

        private void OnStopSpawnPortals(Signals.Args obj) {
            m_isRunning = false;
            m_sequence.Kill();

            var toDespawn = m_portals.Keys.ToList();
            foreach (var at in toDespawn) {
                DespawnPortal(at);
            }
        }

        private void SpawnPortals() {
            if (!m_isRunning) {
                m_sequence.Kill();
                return;
            }

            m_sequence = DOTween.Sequence().AppendCallback(() => {
                var toSpawn = Math.Min(spawns.Length, Random.Range(2, maxPortalSpawn));
                for (int i = 0; i < toSpawn; i++) {
                    var spawnIndex = Random.Range(0, m_freeSpawns.Count);
                    SpawnPortal(spawnIndex);
                }
            }).AppendInterval(waitForDespawn).OnComplete(DespawnPortals);
        }

        private void DespawnPortals() {
            if (!m_isRunning) {
                m_sequence.Kill();
                return;
            }

            m_sequence = DOTween.Sequence().AppendCallback(() => {
                var toDespawn = m_portals.Keys.ToList();
                foreach (var at in toDespawn) {
                    DespawnPortal(at);
                }
            }).AppendInterval(waitForSpawn).OnComplete(SpawnPortals);
        }

        private bool SpawnPortal(int spawnIndex) {
            if (m_occupiedSpawns.Contains(spawnIndex)) return false;

            var obj = Instantiate(prefab, transform);
            if (!obj.TryGetComponent(out Portal portal)) {
                Destroy(obj);
                return false;
            }

            var spawn = spawns[spawnIndex];
            portal.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            portal.Init(this, spawnIndex);
            portal.OnSpawned();

            m_portals.Add(spawnIndex, portal);

            m_freeSpawns.Remove(spawnIndex);
            m_occupiedSpawns.Add(spawnIndex);

            return true;
        }

        private bool DespawnPortal(int spawnIndex) {
            if (!m_portals.TryGetValue(spawnIndex, out Portal portal)) return false;

            portal.OnDespawned();

            m_portals.Remove(spawnIndex);

            m_occupiedSpawns.Remove(spawnIndex);
            m_freeSpawns.Add(spawnIndex);

            //Destroy(portal.gameObject);

            return true;
        }
    }
}