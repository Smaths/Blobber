using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UI_GameOver : MonoBehaviour
    {
        [SerializeField] private GameObject _player;

        [Header("UI Elements")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Fade Time")]
        [SerializeField] private float _fadeInTime = 0.3f;

        private void OnValidate()
        {
            _canvas ??= GetComponent<Canvas>();
            _canvasGroup ??= GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1f, _fadeInTime);
        }

        #region Button Events
        public void Resume_Tapped()
        {
            gameObject.SetActive(false);
        }
        public void Retry_Tapped()
        {
            _canvasGroup.interactable = false;
            SceneFader.instance.FadeToLevel1();
        }

        public void Leaderboard_Tapped()
        {

        }

        public void Quit_Tapped()
        {
            _canvasGroup.interactable = false;
            SceneFader.instance.FadeToStart();
        }
        #endregion

        public void Toggle()
        {
            // Prevent pause menu when game is over.
            if (LevelManager.instance.GameIsOver) return;

            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}