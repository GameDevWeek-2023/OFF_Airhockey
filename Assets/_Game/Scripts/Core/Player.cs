using UnityEngine;

namespace Airhockey.Core {
    public class Player : MonoBehaviour {
        [SerializeField] private bool isLocked;
        private PlayerBehaviour[] m_behaviours;
        private PlayerDetails m_details;

        public int Id { get; private set; } = 0;

        private PlayerBehaviour[] Behaviours =>
            m_behaviours ??= GetComponents<PlayerBehaviour>();

        public bool IsLocked {
            get => isLocked;
            set {
                foreach (var behaviour in Behaviours) {
                    if (value) {
                        behaviour.OnLock();
                    }
                    else {
                        behaviour.OnUnlock();
                    }
                }

                isLocked = value;
            }
        }

        public bool TryGetDetails(out PlayerDetails details) {
            if (m_details == null) {
                details = null;
                return false;
            }

            details = m_details;
            return true;
        }

        public bool Join(int index) {
            Id = index;
            m_details = GameManager.GetPlayerDetails(index);
            return true;
        }
    }
}