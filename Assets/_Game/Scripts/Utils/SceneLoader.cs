using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Airhockey.Utils {
    public class SceneLoader : MonoBehaviour {
        [SerializeField, Scene] private int buildIndex;

        [SerializeField] private float fadeDuration = 3f;
        [SerializeField] private Image fadeImage;

        public void Load() {
            DoFade(() => { SceneManager.LoadScene(buildIndex); });
        }

        public void Quit() {
            DoFade(Application.Quit);
        }

        private void DoFade(TweenCallback onComplete) {
            DOTween.Sequence().Append(fadeImage.DOFade(1.0f, fadeDuration)).OnComplete(onComplete);
        }
    }
}