using UnityEngine;
using Utility;

// Script execution order modified.
namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private bool _isGameOver;
        [SerializeField] private bool _isInKidModeDefault;

        public bool IsGameOver => _isGameOver;

        private void Start()
        {
            BadWordFilter.Load();
        }

        public void SetGameOver()
        {
            _isGameOver = true;

            LootLockerTool.Instance.SubmitPlayerScore(ScoreManager.Instance.Points);
        }
    }
}
