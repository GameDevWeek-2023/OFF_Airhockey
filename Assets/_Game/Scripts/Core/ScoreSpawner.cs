using System;
using System.Collections.Generic;
using Airhockey.Events;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Airhockey.Core {
    public class ScoreSpawner : MonoBehaviour {
        [SerializeField] private int playerIndex = 0;
        [SerializeField] private Vector3 offsetDirection = Vector3.right;
        [SerializeField] private float spacing = 0.25f;
        [SerializeField] private GameObject[] scorePrefabs;
        [SerializeField] private Vector3 punch = Vector3.one;

        private int m_score;
        private List<GameObject> m_scoreObjects = new List<GameObject>();

        private void OnEnable() {
            Signals.Subscribe(GameSignal.OnGoalScored, OnGoalScored);
        }

        private void OnDisable() {
            Signals.Unsubscribe(GameSignal.OnGoalScored, OnGoalScored);
        }

        private void OnGoalScored(Signals.Args args) {
            if (!args.Read(out int index) || index == playerIndex) return;

            var origin = transform.position;
            var dir = transform.TransformDirection(offsetDirection).normalized;

            var position = origin + (dir * (m_score * spacing));
            var obj = Instantiate(scorePrefabs[index], transform);
            obj.transform.SetPositionAndRotation(position, transform.rotation);
            obj.transform.DOPunchScale(punch, 0.5f);

            m_score += 1;
            m_scoreObjects.Add(obj);
        }

        private void OnDrawGizmosSelected() {
            var origin = transform.position;
            var dir = transform.TransformDirection(offsetDirection).normalized;
            Gizmos.DrawLine(origin, origin + dir * 5 * spacing);

            for (int i = 0; i < 5; i++) {
                var position = origin + (dir * (i * spacing));
                Gizmos.DrawSphere(position, 0.025f);
            }
        }
    }
}