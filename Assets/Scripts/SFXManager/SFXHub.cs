using Sirenix.OdinInspector;
using UnityEngine;

namespace SFXManager
{
    public class SFXHub : MonoBehaviour
    {
        [Space]
        [TitleGroup("SFX Connection Hub", "Connect SFX to all game events (in one convenient location!) :D")]

        [BoxGroup("SFX Connection Hub/Score")]
        [SerializeField] private SFX OnScoreDecreaseSFX;
        [BoxGroup("SFX Connection Hub/Score")]
        [SerializeField] private SFX OnScoreIncreaseSFX;
        [BoxGroup("SFX Connection Hub/Score")]
        [SerializeField] private SFX OnDeadSFX;

        [BoxGroup("SFX Connection Hub/Player")]
        [SerializeField] private SFX OnBoostActivatedSFX;

        [BoxGroup("SFX Connection Hub/Game Time")]
        [SerializeField] private SFX OnGamePauseSFX;
        [BoxGroup("SFX Connection Hub/Game Time")]
        [SerializeField] private SFX OnGameResumeSFX;
        [BoxGroup("SFX Connection Hub/Game Time")]
        [SerializeField] private SFX OnGameStartSFX;
        [BoxGroup("SFX Connection Hub/Game Time")]
        [SerializeField] private SFX OnGameOverSFX;

        #region Score Manager
        public void OnScoreDecreaseEvent()
        {
            if (isActiveAndEnabled)
                OnScoreDecreaseSFX?.PlaySFX();
        }

        public void OnScoreIncreaseEvent()
        {
            if (isActiveAndEnabled)
                OnScoreIncreaseSFX?.PlaySFX();
        }

        public void OnScoreIsZero()
        {
            if (isActiveAndEnabled)
                OnDeadSFX?.PlaySFX();
        }
        #endregion

        #region Player
        public void OnBoostActivatedEvent()
        {
            if (isActiveAndEnabled)
                OnBoostActivatedSFX?.PlaySFX();
        }
        #endregion

        #region Game Timer
        public void OnGameStartEvent()
        {
            if (isActiveAndEnabled)
                OnGameStartSFX?.PlaySFX();
        }

        public void OnGamePauseEvent()
        {
            if (isActiveAndEnabled)
                OnGamePauseSFX?.PlaySFX();
        }

        public void OnGameResumeEvent()
        {
            if (isActiveAndEnabled)
                OnGameResumeSFX?.PlaySFX();
        }

        public void OnGameOverEvent()
        {
            if (isActiveAndEnabled)
                OnGameOverSFX?.PlaySFX();
        }
        #endregion
    }
}