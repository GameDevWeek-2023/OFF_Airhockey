using System;
using Airhockey.Events;
using UnityEngine;

namespace Airhockey.Core {
    public class Player : MonoBehaviour {
        [SerializeField] private bool isLocked;
        private PlayerBehaviour[] m_behaviours;

        public int Id { get; private set; }

        private PlayerBehaviour[] Behaviours =>
            m_behaviours ??= GetComponents<PlayerBehaviour>();

        private void OnEnable() {
            Signals.Subscribe(GameSignal.OnGoalScored, OnGoalScored);
        }

        private void OnDisable() {
            Signals.Unsubscribe(GameSignal.OnGoalScored, OnGoalScored);
        }

        public bool IsLocked {
            get => isLocked;
            set => isLocked = value;
        }

        public bool Join(int index) {
            Id = index;
            return true;
        }

        private void OnGoalScored(Signals.Args obj) {
            IsLocked = true;
        }
    }
}