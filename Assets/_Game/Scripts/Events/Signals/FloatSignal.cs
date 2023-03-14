using System;
using UnityEngine;

namespace Airhockey.Events.Signals {
    [CreateAssetMenu(fileName = "Float Signal", menuName = "Signals/Float", order = 0)]
    public class FloatSignal : ValueSignal<float> {
        private event Action<float> OnPerformed;

        public override void Publish(float value) {
            OnPerformed?.Invoke(value);
        }

        public override void Subscribe(Action<float> subscriber) {
            OnPerformed += subscriber;
        }

        public override void Unsubscribe(Action<float> subscriber) {
            OnPerformed -= subscriber;
        }
    }
}