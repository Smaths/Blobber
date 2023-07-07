using System.Globalization;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Random = UnityEngine.Random;

// Script execution order modified.

namespace Managers
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        #region Fields
        [Title("Score Manager", "Main brain for handling score and publishing related events.")]
        // Editor fields
        [Tooltip("Current points of the player, game over if points go below 0.")]
        [SerializeField] private int _points;
        [SerializeField, ReadOnly] private bool _gameIsOver;

        [BoxGroup("In-Game Popup")][SerializeField] private Canvas _canvas;
        [BoxGroup("In-Game Popup")][SerializeField] private GameObject _popupPrefab;
        [SuffixLabel("second(s)"), MinValue(0.25f)]
        [BoxGroup("In-Game Popup")][SerializeField] private float _popupDuration = 3f;

        private Vector3 _popupLocation;
        private Camera _camera;
        private readonly Color _goodColor = new (0.749f, 0.753f, 0.247f, 1.0f);
        private readonly Color _badColor = new (0.682f, 0.298f, 0.294f, 1.0f);
        #endregion

        #region Public Properties
        public int Points => _points;
        public bool GameIsOver => _gameIsOver;
        #endregion

        #region Events
        [FoldoutGroup("Events", false)] public UnityEvent<int, int> ScoreChanged;   // Amount changed, new total score
        [FoldoutGroup("Events")] public UnityEvent<int> OnScoreIncrease;
        [FoldoutGroup("Events")] public UnityEvent<int> OnScoreDecrease;
        [FoldoutGroup("Events")] public UnityEvent OnScoreIsZero;
        #endregion

        #region Lifecycle
        private void Start()
        {
            _camera = Camera.main;
        }
        #endregion

        #region Modify Score
        // More generic method without dependency on `Blob` class
        public void AddPoints(int value, Vector3 popupLocation)
        {
            AddPoints(value);
            CreateScorePopup(value, popupLocation);
        }

        private void AddPoints(int value)
        {
            _points += value;

            switch (value)
            {
                case > 0:
                    OnScoreIncrease?.Invoke(value);
                    break;
                case < 0:
                    OnScoreDecrease?.Invoke(value);
                    break;
            }

            ScoreChanged?.Invoke(value, _points);

            // End game
            if (_points <= 0)
            {
                _points = 0;
                OnScoreIsZero?.Invoke();
            }
        }
        #endregion

        // Popup
        private void CreateScorePopup(int value, Vector3 worldPosition)
        {
            GameObject popUp = Instantiate(_popupPrefab, _canvas.transform);

            // Set text
            var label = popUp.GetComponent<TMP_Text>();
            label.text = value.ToString(CultureInfo.CurrentCulture);

            // Set location
            Vector3 rectLocation = _camera.WorldToScreenPoint(worldPosition);
            var rectTransform = popUp.GetComponent<RectTransform>();
            rectTransform.position = rectLocation;

            // Animate
            float randomX = Random.Range(-200f, 200f);
            rectTransform.DOLocalMove(new Vector3(randomX, 200f, 1), _popupDuration)
                .SetRelative(true)
                .SetEase(Ease.OutQuint);
            rectTransform.DOPunchScale(Vector3.one * 1.5f, _popupDuration, 4, 0.25f);
            label.color = value > 0 ? _goodColor : _badColor;

            Destroy(popUp, _popupDuration);
        }
    }
}