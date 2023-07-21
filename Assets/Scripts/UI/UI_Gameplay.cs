using DG.Tweening;
using Managers;
using UnityEngine;

namespace UI
{
    public class UI_Gameplay : MonoBehaviour
    {
        [SerializeField] private float _animationTime = 0.4f;

        [SerializeField] private CanvasGroup[] _canvasGroups;

        private void OnEnable()
        {
            _canvasGroups ??= GetComponentsInChildren<CanvasGroup>();

            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnPreCountdownStarted.AddListener(Hide);
                GameTimer.Instance.OnCountdownStarted.AddListener(Show);
                GameTimer.Instance.OnCountdownCompleted.AddListener(Hide);
                GameTimer.Instance.OnPause.AddListener(Hide);
                GameTimer.Instance.OnResume.AddListener(Show);
            }

            if (ScoreManager.instanceExists) ScoreManager.Instance.OnScoreIsZero.AddListener(Hide);
        }

        private void Start()
        {
            // Hide game UI's on start
            foreach (CanvasGroup canvasGroup in _canvasGroups)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
            }
        }

        private void OnDestroy()
        {
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnPreCountdownStarted.RemoveListener(Hide);
                GameTimer.Instance.OnCountdownStarted.RemoveListener(Show);
                GameTimer.Instance.OnCountdownCompleted.RemoveListener(Hide);
                GameTimer.Instance.OnPause.RemoveListener(Hide);
                GameTimer.Instance.OnResume.RemoveListener(Show);
            }

            if (ScoreManager.instanceExists) ScoreManager.Instance.OnScoreIsZero.RemoveListener(Hide);
        }

        private void Show()
        {
            foreach (CanvasGroup canvasGroup in _canvasGroups)
            {
                canvasGroup.interactable = true;
                canvasGroup.DOFade(1, _animationTime)
                    .OnComplete(() => canvasGroup.interactable = true);
            }
        }

        private void Hide()
        {
            foreach (CanvasGroup canvasGroup in _canvasGroups)
            {
                if (_animationTime > 0)
                {
                    canvasGroup.DOFade(0, _animationTime)
                        .OnComplete(() => canvasGroup.interactable = false);
                }
                else
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                }
            }
        }
    }
}