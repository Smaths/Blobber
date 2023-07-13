using UnityEngine;
using Utility;

// Script execution order modified.
namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private bool _isGameOver;
        public bool IsGameOver => _isGameOver;

        public void SetGameOver()
        {
            _isGameOver = true;

            LootLockerTool.Instance.SubmitPlayerScore(ScoreManager.Instance.Points);
        }
    }
}
