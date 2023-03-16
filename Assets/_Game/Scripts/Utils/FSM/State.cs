using Unity.VisualScripting;
using UnityEngine;

namespace Airhockey.Utils.FSM {
    public abstract class State<TMachine, TParent>
        where TParent : MonoBehaviour where TMachine : StateMachine<TMachine, TParent> {
        private TMachine m_stateMachine;
        private TParent m_parent;

        protected TMachine StateMachine => m_stateMachine;
        protected TParent Parent => m_parent;

        public void Init(TMachine machine) {
            m_stateMachine = machine;
            m_parent = machine.Parent;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();
    }
}