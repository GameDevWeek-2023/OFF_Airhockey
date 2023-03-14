using System;
using UnityEngine;

namespace Airhockey.Events {
    public abstract class ValueSignal<TValue> : BaseSignal {
        public abstract void Publish(TValue value);
        public abstract void Subscribe(Action<TValue> subscriber);
        public abstract void Unsubscribe(Action<TValue> subscriber);
    }
}