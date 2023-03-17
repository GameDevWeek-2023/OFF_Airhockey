using System;
using Airhockey.Utils;
using UnityEngine;

namespace Airhockey.Core {
    [Serializable]
    public class PlayerDetails {
        public string name;
        public Color color;
    }

    public class GameManager : MonoSingleton<GameManager> {
        [SerializeField] private PlayerDetails[] playerDetails;

        public static PlayerDetails GetPlayerDetails(int index) {
            return Instance.playerDetails[index];
        }
    }
}