using UnityEngine;
using UnityEngine.InputSystem;

namespace Airhockey.Core {
    public class PlayerSlowmo : PlayerBehaviour {
        private bool m_isPerformingSlowmo;

        public void OnSlowmo(InputAction.CallbackContext ctx) {
            if (ctx.performed && !m_isPerformingSlowmo) {
                m_isPerformingSlowmo = SlowmoManager.StartSlowmo(Player.Id);
            }
            else if (ctx.canceled) {
                m_isPerformingSlowmo = !SlowmoManager.StopSlowmo(Player.Id);
            }
        }
    }
}