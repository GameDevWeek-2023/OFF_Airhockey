namespace Airhockey.Utils.FSM {
    public abstract class State {
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();
    }
}