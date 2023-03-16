using UnityEngine;

namespace Airhockey.Core {
    public class Player : MonoBehaviour {
        [SerializeField] private bool isLocked;
        private PlayerBehaviour[] m_behaviours;

        public int Id { get; private set; }

        public GameManager.PlayerDetails Details { get; private set; }

        private PlayerBehaviour[] Behaviours =>
            m_behaviours ??= GetComponents<PlayerBehaviour>();

        public bool IsLocked {
            get => isLocked;
            set {
                IsLocked = value;
                foreach (var behaviour in m_behaviours) {
                    if (value)
                        behaviour.OnLock();
                    else
                        behaviour.OnLock();
                }
            }
        }

        public bool Join(int index) {
            Id = index;
            Details = GameManager.GetPlayerDetails(index);
            return true;
        }
    }
}