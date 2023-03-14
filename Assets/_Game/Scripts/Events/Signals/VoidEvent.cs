using System;
using UnityEngine;

namespace Airhockey.Events.Signals{
    [CreateAssetMenu(fileName = "Void Event", menuName = "Signals/Void", order = 0)]
    public class VoidEvent : Signal {
        private event Action OnPerformed;

        public override void Publish() {
            OnPerformed?.Invoke();
        }

        public override void Subscribe(Action subscriber) {
            OnPerformed += subscriber;
        }

        public override void Unsubscribe(Action subscriber) {
            OnPerformed -= subscriber;
        }
    }
}