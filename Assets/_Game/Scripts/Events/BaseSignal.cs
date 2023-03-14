using UnityEngine;

namespace Airhockey.Events {
    public abstract class BaseSignal : ScriptableObject {
        [SerializeField] private new string name = "signal";
        [SerializeField, TextArea] private string description;
    }
}