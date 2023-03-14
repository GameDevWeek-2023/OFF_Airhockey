using System;
using UnityEngine;

namespace Airhockey.Events {
    public abstract class Signal : BaseSignal {
        public abstract void Publish();
        public abstract void Subscribe(Action subscriber);
        public abstract void Unsubscribe(Action subscriber);
    }
}