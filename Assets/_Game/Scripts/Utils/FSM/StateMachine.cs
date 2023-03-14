using System.Collections.Generic;

namespace Airhockey.Utils.FSM {
    public class StateMachine<TKey> {
        private readonly Dictionary<TKey, State> m_states;
        private State m_currentState;

        public StateMachine() {
            m_states = new Dictionary<TKey, State>();
        }

        public void Entry(TKey key) {
            if (!TryGetState(key, out State state)) return;

            m_currentState = state;
            m_currentState.OnEnter();
        }

        public bool AddState(TKey key, State state) {
            if (!Contains(key)) return false;

            m_states.Add(key, state);
            return true;
        }

        public bool RemoveState(TKey key) {
            if (Contains(key)) return false;

            m_states.Remove(key);
            return true;
        }

        public bool Contains(TKey key) {
            return m_states.ContainsKey(key);
        }

        public bool TryGetState(TKey key, out State state) {
            return m_states.TryGetValue(key, out state);
        }

        public bool SwitchState(TKey key) {
            if (!TryGetState(key, out State state)) return false;

            m_currentState?.OnExit();
            m_currentState = state;
            m_currentState?.OnEnter();
            return false;
        }

        public void Update() {
            m_currentState?.OnUpdate();
        }
    }
}