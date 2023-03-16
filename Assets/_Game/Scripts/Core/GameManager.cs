using System;
using Airhockey.Utils;
using UnityEngine;

namespace Airhockey.Core {
    public class GameManager : MonoSingleton<GameManager> {
        [Serializable]
        public class PlayerDetails {
            public string name;
            public Color color;
        }

        [SerializeField] private PlayerDetails[] playerDetails;

        public static PlayerDetails GetPlayerDetails(int index) {
            return Instance.playerDetails[index];
        }
    }
}