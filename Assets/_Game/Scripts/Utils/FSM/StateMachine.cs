using System.Collections.Generic;
using UnityEngine;

namespace Airhockey.Utils.FSM {
    public class StateMachine<TMachine, TParent> where TParent : MonoBehaviour
        where TMachine : StateMachine<TMachine, TParent> {
        private readonly Dictionary<string, State<TMachine, TParent>> m_states;
        private State<TMachine, TParent> m_currentState;

        public TParent Parent { get; }

        public StateMachine(TParent parent) {
            Parent = parent;

            m_states = new Dictionary<string, State<TMachine, TParent>>();
        }

        public bool Entry(string key) {
            if (!m_states.TryGetValue(key, out State<TMachine, TParent> state)) return false;

            m_currentState = state;
            m_currentState?.OnEnter();
            return true;
        }

        public bool AddState(string key, State<TMachine, TParent> state) {
            if (m_states.ContainsKey(key)) return false;

            m_states.Add(key, state);
            state.Init(this as TMachine);
            return true;
        }

        public bool RemoveState(string key) {
            if (!m_states.ContainsKey(key)) return false;

            m_states.Remove(key);
            return true;
        }

        public void SwitchState(string key) {
            if (!m_states.TryGetValue(key, out State<TMachine, TParent> state)) return;

            m_currentState.OnExit();
            m_currentState = state;
            m_currentState.OnEnter();
        }

        public void Update() {
            m_currentState?.OnUpdate();
        }
    }
}