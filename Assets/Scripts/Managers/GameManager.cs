using UnityEngine;
using Utility;

// Script execution order modified.
namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private bool _isGameOver;

        public bool IsGameOver
        {
            get => _isGameOver;
            set
            {
                _isGameOver = value;
                LootLockerTool.Instance.SubmitPlayerScore(ScoreManager.Instance.Points);
            }
        }

        #region Lifecycle
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        #endregion
    }
}
