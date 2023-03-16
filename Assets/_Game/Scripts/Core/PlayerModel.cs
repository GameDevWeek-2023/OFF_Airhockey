using System;
using UnityEngine;

namespace Airhockey.Core {
    public class PlayerModel : PlayerBehaviour {
        [SerializeField] private GameObject[] prefabs;
        private GameObject m_model;

        private void OnEnable() {
            if (prefabs.Length <= 0) return;

            m_model = Instantiate(prefabs[Player.Id], transform);
        }

        public void Clear() {
            if (m_model == null) return;

            Destroy(m_model);
        }

    }
}