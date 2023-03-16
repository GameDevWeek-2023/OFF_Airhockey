using UnityEngine;

namespace Airhockey.Core {
    public class PlayerBehaviour : MonoBehaviour {
        private Player m_player;
        protected Player Player => m_player ??= GetComponent<Player>();

        protected bool IsLocked => Player.IsLocked;
    }
}