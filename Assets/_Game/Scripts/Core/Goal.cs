using Airhockey.Events;
using Airhockey.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Airhockey.Core {
    public class Goal : MonoBehaviour {
        [SerializeField] private int index = 0;
        [SerializeField] private LayerMask mask;
        [SerializeField] private UnityEvent onGoal;

        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.layer.IsInLayerMask(mask)) return;
            
            Signals.Publish(GameSignal.OnGoalScored, index);
            onGoal?.Invoke();
        }
    }
}