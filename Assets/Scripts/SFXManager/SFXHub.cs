using Sirenix.OdinInspector;
using UnityEngine;

namespace SFXManager
{
    public class SFXHub : MonoBehaviour
    {
        [Space]
        [Title("SFX Connection Hub", "Connect SFX to all game events (in one convenient location!) :D")]
        [Title("Score")]
        [SerializeField] private SFX OnScoreDecreaseSFX;
        [SerializeField] private SFX OnScoreIncreaseSFX;
        [SerializeField] private SFX OnDeadSFX;
        [Title("Player")]
        [SerializeField] private SFX OnBoostActivatedSFX;
        [Title("Game Time")]
        [SerializeField] private SFX OnGamePauseSFX;
        [SerializeField] private SFX OnGameResumeSFX;
        [SerializeField] private SFX OnGameStartSFX;
        [SerializeField] private SFX OnGameOverSFX;

        #region Score Manager
        public void OnScoreDecreaseEvent()
        {
            OnScoreDecreaseSFX?.PlaySFX();
        }

        public void OnScoreIncreaseEvent()
        {
            OnScoreIncreaseSFX?.PlaySFX();
        }

        public void OnScoreIsZero()
        {
            OnDeadSFX?.PlaySFX();
        }
        #endregion

        #region Player
        public void OnBoostActivatedEvent()
        {
            OnBoostActivatedSFX?.PlaySFX();
        }
        #endregion

        #region Game Timer
        public void OnGameStartEvent()
        {
            OnGameStartSFX?.PlaySFX();
        }

        public void OnGamePauseEvent()
        {
            OnGamePauseSFX?.PlaySFX();
        }

        public void OnGameResumeEvent()
        {
            OnGameResumeSFX?.PlaySFX();
        }

        public void OnGameOverEvent()
        {
            OnGameOverSFX?.PlaySFX();
        }
        #endregion
    }
}