using UnityEngine;
using Utility;

// Script execution order modified.
namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private bool _isGameOver;

        public bool IsGameOver => _isGameOver;

        #region Lifecycle
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnCountdownCompleted.AddListener(SetGameOver);
            }

            if (ScoreManager.instanceExists)
            {
                ScoreManager.Instance.OnScoreIsZero.AddListener(SetGameOver);
            }
        }

        private void OnDisable()
        {
            if (GameTimer.instanceExists)
            {
                GameTimer.Instance.OnCountdownCompleted.RemoveListener(SetGameOver);
            }

            if (ScoreManager.instanceExists)
            {
                ScoreManager.Instance.OnScoreIsZero.RemoveListener(SetGameOver);
            }
        }
        #endregion

        private void SetGameOver()
        {
            _isGameOver = true;

            LootLockerTool.Instance.SubmitPlayerScore(ScoreManager.Instance.Points);
        }
    }
}
