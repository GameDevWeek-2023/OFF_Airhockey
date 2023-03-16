using System;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Airhockey.Events {
    public static class Signals {
        public readonly struct Args {
            private readonly object[] values;

            public Args(params object[] values) {
                this.values = values ?? Array.Empty<object>();
            }

            public bool Read<TValue>(int index, out TValue value) {
                value = (TValue)values[index];
                return values[index] is TValue;
            }

            public bool Read<TValue>(out TValue value) {
                return Read(0, out value);
            }

            public override string ToString() {
                var str = "";
                foreach (var value in values) {
                    str += $"{Convert.ChangeType(value, value.GetType())}, ";
                }

                return str;
            }
        }

        private class Event {
            public event Action<Args> Performed;

            public void Invoke(params object[] args) {
                Performed?.Invoke(new Args(args));
            }

            public void Invoke() {
                Performed?.Invoke(new Args(null));
            }
        }

        private static class Events<TKey> {
            private static readonly Dictionary<TKey, Event> Actions = new Dictionary<TKey, Event>();

            public static Event Get(TKey key) {
                if (Actions.TryGetValue(key, out Event evt)) return evt;

                evt = new Event();
                evt.Performed += args => { Debug.Log($"[Signals]: Event {key}, Value: {args}"); };
                Actions.Add(key, evt);

                return evt;
            }
        }

        public static void Publish<TKey>(TKey key, params object[] args) {
            var evt = Events<TKey>.Get(key);
            evt.Invoke(args);
        }

        public static void Publish<TKey>(TKey key) {
            var evt = Events<TKey>.Get(key);
            evt.Invoke();
        }

        public static void PublishWithDelay<TKey>(TKey key, int delay = 1000, params object[] args) {
            Thread.Sleep(delay);
            Publish(key, args);
        }

        public static void PublishWithDelay<TKey>(TKey key, int delay = 1000) {
            Thread.Sleep(delay);
            Publish(key);
        }

        public static void Subscribe<TKey>(TKey key, Action<Args> subscriber) {
            var evt = Events<TKey>.Get(key);
            evt.Performed += subscriber;
        }

        public static void Unsubscribe<TKey>(TKey key, Action<Args> subscriber) {
            var evt = Events<TKey>.Get(key);
            evt.Performed -= subscriber;
        }
    }
}