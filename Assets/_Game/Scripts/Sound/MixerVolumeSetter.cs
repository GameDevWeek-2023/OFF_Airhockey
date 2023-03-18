using Airhockey.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Airhockey.Sound {
    [RequireComponent(typeof(Slider))]
    public class MixerVolumeSetter : MonoBehaviour {
        [SerializeField] private string group = "Mixer_Group_Name";
        private SoundManager m_soundManager;
        private Slider m_slider;

        private SoundManager SoundManager =>
            m_soundManager ??= SoundManager.Instance;

        private Slider Slider => m_slider != null ? m_slider : (m_slider = GetComponent<Slider>());

        private void Start() {
            if (!SoundManager.GetGroupVolume(@group, out float value)) return;

            Slider.value = value;
        }

        private void OnEnable() {
            Slider.onValueChanged.AddListener(ValueChangeCallback);
        }

        private void OnDisable() {
            Slider.onValueChanged.RemoveListener(ValueChangeCallback);
        }


        private void ValueChangeCallback(float value) {
            SoundManager.SetGroupVolume(group, value);
        }
    }
}