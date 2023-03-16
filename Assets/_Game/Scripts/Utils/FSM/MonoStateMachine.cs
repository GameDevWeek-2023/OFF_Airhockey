using System;
using System.Collections.Generic;
using UnityEngine;

namespace Airhockey.Utils.FSM {
    public abstract class MonoStateMachine<TKey> : MonoBehaviour {
        private readonly Dictionary<TKey, MonoState<TKey>> m_states = new Dictionary<TKey, MonoState<TKey>>();
        private MonoState<TKey> m_currentState;

        private void Awake() {
            foreach (var state in GetComponents<MonoState<TKey>>()) {
                AddState(state);
            }
        }

        public bool SwitchState(TKey stateId) {
            if (!m_states.TryGetValue(stateId, out MonoState<TKey> state)) return false;

            m_currentState.OnExit(this);
            m_currentState = state;
            m_currentState.OnEnter(this);

            return true;
        }

        public bool AddState(MonoState<TKey> state) {
            if (m_states.ContainsKey(state.Id())) return false;

            m_states.Add(state.Id(), state);
            return true;
        }

        public bool RemoveState(MonoState<TKey> state) {
            if (!m_states.ContainsKey(state.Id())) return false;

            m_states.Remove(state.Id());
            return true;
        }
    }
}