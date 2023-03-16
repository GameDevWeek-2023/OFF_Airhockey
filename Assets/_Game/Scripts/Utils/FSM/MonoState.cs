using UnityEngine;

namespace Airhockey.Utils.FSM {
    public abstract class MonoState<TKey> : MonoBehaviour {
        public abstract TKey Id();
        public abstract void OnEnter(MonoStateMachine<TKey> machine);
        public abstract void OnExit(MonoStateMachine<TKey> machine);
    }
}