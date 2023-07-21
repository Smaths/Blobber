using UnityEngine;
using Utility;

// Script execution order modified.
namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private bool _isGameOver;

        public bool IsGameOver => _isGameOver;

        public void SetGameOver(bool value = true)
        {
            _isGameOver = value;

            LootLockerTool.Instance.SubmitPlayerScore(ScoreManager.Instance.Points);
        }
    }
}
