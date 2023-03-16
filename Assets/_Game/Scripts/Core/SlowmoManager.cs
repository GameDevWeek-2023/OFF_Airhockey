using System;
using Airhockey.Events;
using Airhockey.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Airhockey.Core {
    public class SlowmoManager : MonoSingleton<SlowmoManager> {
        [SerializeField] private float timescale = 0.5f;
        private bool m_isSlowmo = false;
        private int m_playerIndex = -1;
        private float m_defaultTimeScale = 1.0f;
        private float m_defaultFixedDeltaTime = 0.0f;

        private void Awake() {
            m_defaultTimeScale = Time.timeScale;
            m_defaultFixedDeltaTime = Time.fixedDeltaTime;
        }

        public static bool StartSlowmo(int playerIndex) {
            if (Instance.m_isSlowmo) return false;

            Instance.m_playerIndex = playerIndex;
            Time.timeScale = Instance.timescale;
            Time.fixedDeltaTime = Instance.m_defaultFixedDeltaTime * Time.timeScale;
            Instance.m_isSlowmo = true;

            Signals.Publish(GameSignal.OnSlowmoStart, playerIndex);
            return true;
        }


        public static bool StopSlowmo(int playerIndex) {
            if (!Instance.m_isSlowmo || playerIndex != Instance.m_playerIndex) return false;

            Time.timeScale = Instance.m_defaultTimeScale;
            Time.fixedDeltaTime = Instance.m_defaultFixedDeltaTime * Time.timeScale;

            Instance.m_playerIndex = -1;
            Instance.m_isSlowmo = false;
            Signals.Publish(GameSignal.OnSlowmoEnd, playerIndex);

            return true;
        }
    }
}