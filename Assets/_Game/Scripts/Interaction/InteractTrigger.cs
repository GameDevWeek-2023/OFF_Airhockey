using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Airhockey.Interaction {
    public class InteractTrigger : MonoBehaviour, IInteract {
        [Foldout("Events")] public UnityEvent onHoverEnter, onHoverExit;
        [Foldout("Events")] public UnityEvent<int> onInteract;

        public void OnHoverEnter() {
            onHoverEnter?.Invoke();
        }

        public void OnHoverExit() {
            onHoverExit?.Invoke();
        }

        public void OnInteract(int button) {
            onInteract?.Invoke(button);
        }
    }
}