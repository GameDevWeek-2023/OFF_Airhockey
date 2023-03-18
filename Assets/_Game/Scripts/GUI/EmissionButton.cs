using DG.Tweening;
using UnityEngine;

namespace Airhockey.GUI {
    public class EmissionButton : MonoBehaviour {
        [SerializeField] private MeshRenderer renderer;
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private Color onHoverEnterColor, onHoverExitColor;
        [SerializeField] private Ease ease;


        public void OnHoverEnter() {
            renderer.material.DOColor(onHoverEnterColor, "_EmissionColor", duration).SetEase(ease);
        }

        public void OnHoverExit() {
            renderer.material.DOColor(onHoverExitColor, "_EmissionColor", duration).SetEase(ease);
        }

        public void OnInteract(int button) { }
    }
}