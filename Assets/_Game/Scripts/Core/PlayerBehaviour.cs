using UnityEngine;

namespace Airhockey.Core {
    public abstract class PlayerBehaviour : MonoBehaviour {
        private Player m_player;
        protected Player Player => m_player ??= GetComponent<Player>();

        protected bool IsLocked => Player.IsLocked;

        public virtual void OnLock() { }
        public virtual void OnUnlock() { }
    }
}