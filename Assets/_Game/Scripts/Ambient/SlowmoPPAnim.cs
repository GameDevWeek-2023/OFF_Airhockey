using Airhockey.Ambient;
using Airhockey.Core;
using Airhockey.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace _Game.Scripts.Ambient {
    public class SlowmoPPAnim : PostProcessAnim {
        [SerializeField] private float chromaticAberrationIntensity = 0.25f;
        [SerializeField] private float vignetteIntensity = 0.45f;

        [SerializeField] private float duration = 0.1f;

        private void OnEnable() {
            Signals.Subscribe(GameSignal.OnSlowmoStart, OnSlowmoStart);
            Signals.Subscribe(GameSignal.OnSlowmoEnd, OnSlowmoEnd);
        }

        private void OnDisable() {
            Signals.Unsubscribe(GameSignal.OnSlowmoStart, OnSlowmoStart);
            Signals.Unsubscribe(GameSignal.OnSlowmoEnd, OnSlowmoEnd);
        }

        private void OnSlowmoStart(Signals.Args obj) {
            var profile = Volume.profile;

            if (!profile.TryGetSettings(out ChromaticAberration chromaticAberration) ||
                !profile.TryGetSettings(out Vignette vignette)) return;

            if (obj.Read(out int index)) {
                var color = GameManager.GetPlayerDetails(index).color;
                vignette.color.value = color;
            }

            DOTween.Sequence().Append(DOTween.To(() => chromaticAberration.intensity.value,
                value => chromaticAberration.intensity.value = value, chromaticAberrationIntensity, duration)).Insert(0,
                DOTween.To(() => vignette.intensity.value,
                    value => vignette.intensity.value = value, vignetteIntensity, duration));
        }

        private void OnSlowmoEnd(Signals.Args obj) {
            var profile = Volume.profile;

            if (!profile.TryGetSettings(out ChromaticAberration chromaticAberration) ||
                !profile.TryGetSettings(out Vignette vignette)) return;


            DOTween.Sequence().Append(DOTween.To(() => chromaticAberration.intensity.value,
                value => chromaticAberration.intensity.value = value, 0.0f, duration)).Insert(0,
                DOTween.To(() => vignette.intensity.value,
                    value => vignette.intensity.value = value, 0.0f, duration));
        }
    }
}