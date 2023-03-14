using UnityEngine;

namespace Airhockey.Events.Trigger {
    public abstract class SignalTrigger<TSignal> : MonoBehaviour where TSignal : BaseSignal {
        protected TSignal signal;

        public bool IsValid => signal != null;
    }
}