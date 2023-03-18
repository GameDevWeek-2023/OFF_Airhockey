using System;
using System.Linq;
using Airhockey.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Airhockey.Core {
    [RequireComponent(typeof(Arena))]
    public class Match : MonoBehaviour {
        [SerializeField] private int maxGoals = 3;
        [SerializeField] private float countdown;
        public UnityEvent onMatchEnd;

        private int[] m_goals;
        private Round m_round;
        private Arena m_arena;

        public Arena Arena => m_arena ??= GetComponent<Arena>();
        public float Countdown => countdown;
        public int MaxGoals => maxGoals;

        private void Awake() {
            m_arena = GetComponent<Arena>();
            m_round = new Round(this);
        }

        private void OnEnable() {
            Signals.Subscribe(GameSignal.OnGoalScored, OnGoalScored);
            Signals.Subscribe(GameSignal.OnMatchEnd, OnMatchEnd);
        }

        private void OnDisable() {
            Signals.Unsubscribe(GameSignal.OnGoalScored, OnGoalScored);
            Signals.Unsubscribe(GameSignal.OnMatchEnd, OnMatchEnd);
        }

        private void OnMatchEnd(Signals.Args args) {
            onMatchEnd?.Invoke();
        }

        private void OnGoalScored(Signals.Args obj) {
            if (!obj.Read(out int index)) return;

            m_goals ??= new int[m_arena.PlayerCount];
            m_goals[index] += 1;
        }

        public void Begin() {
            m_round.Start();
        }

        public bool DidPlayerWon() {
            var max = m_goals.Max();
            return max >= maxGoals;
        }
    }
}