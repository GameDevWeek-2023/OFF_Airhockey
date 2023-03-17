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

        private readonly Dictionary<int, Portal> m_portals = new Dictionary<int, Portal>();
        public Dictionary<int, Portal> Portals => m_portals;
        private bool m_isRunning = false;
        private Sequence m_sequence;

        public Portal GetNextPortal(int start) {
            if (!Portals.ContainsKey(start)) return null;

            var keys = Portals.Keys.ToList();
            keys.Remove(start);

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
        }

        private void SpawnPortals() {
            if (!m_isRunning) {
                m_sequence.Kill();
                return;
            }

            m_sequence = DOTween.Sequence().AppendCallback(() => {
                var toSpawn = Math.Min(spawns.Length, Random.Range(2, maxPortalSpawn));
                for (int i = 0; i < toSpawn;) {
                    var pos = Random.Range(0, spawns.Length);
                    if (m_portals.ContainsKey(pos)) {
                        continue;
                    }

                    SpawnPortal(pos);
                    i++;
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

        private bool SpawnPortal(int at) {
            if (m_portals.ContainsKey(at)) return false;

            var obj = Instantiate(prefab, transform);
            if (!obj.TryGetComponent(out Portal portal)) {
                Destroy(obj);
                return false;
            }

            var spawn = spawns[at];
            portal.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            portal.Init(this, at);
            portal.OnSpawned();

            m_portals.Add(at, portal);

            return true;
        }

        private bool DespawnPortal(int at) {
            if (!m_portals.TryGetValue(at, out Portal portal)) return false;

            portal.OnDespawned();
            m_portals.Remove(at);

            //Destroy(portal.gameObject);

            return true;
        }
    }
}