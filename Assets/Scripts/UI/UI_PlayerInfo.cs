using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    public class UI_PlayerInfo : MonoBehaviour
    {
        [Required] [ChildGameObjectsOnly]
        [SerializeField] private TMP_Text _playerLabel;
        [Required] [ChildGameObjectsOnly]
        [SerializeField] private TMP_Text _rankValueLabel;
        [Required] [ChildGameObjectsOnly]
        [SerializeField] private TMP_Text _scoreLabel;
        [Required] [ChildGameObjectsOnly]
        [SerializeField] private TMP_Text _scoreValueLabel;

        #region Lifecycle
        private void OnEnable()
        {
            SetLabels();

            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnPlayerDataUpdated.AddListener(OnPlayerDataUpdated);
            }
            else
            {
                Debug.Log("Lootlocker missing, no player data found.");
            }
        }

        private void OnDisable()
        {
            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnPlayerDataUpdated.RemoveListener(OnPlayerDataUpdated);
            }
        }
        #endregion

        private void OnPlayerDataUpdated()
        {
            SetLabels();

            Debug.Log($"{gameObject.name} - Animation On Rank Should happen");

            // Perform rank animation during gameplay
            if (!ScoreManager.instanceExists) return;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(3f);
            sequence.Append(_rankValueLabel.DOScale(0, 0.4f)
                .OnComplete(() => _rankValueLabel.text = LootLockerTool.Instance.PrettyRank));
            sequence.Append(_rankValueLabel.DOScale(1, 0.4f));
            sequence.Append(_rankValueLabel.transform.DOPunchScale(Vector3.one * 0.5f, 3, 3));
            sequence.Play();
        }

        private void SetLabels()
        {
            SetName();
            SetRank();
            SetScore();
        }

        private void SetScore()
        {
            _scoreLabel.text = ScoreManager.instanceExists ? "Prev Best" : "Best";
            _scoreValueLabel.text = LootLockerTool.Instance.PrettyHighScore;
        }

        private void SetRank()
        {
            if (LootLockerTool.Instance.Rank <= 0)
                _rankValueLabel.text = "None";
            else
                _rankValueLabel.text = LootLockerTool.Instance.PrettyRank;
        }

        private void SetName()
        {
            if (string.IsNullOrEmpty(LootLockerTool.Instance.PlayerName))
            {
                _playerLabel.text = "Unnamed";
                _playerLabel.alpha = 0.8f;
            }
            else
            {
                _playerLabel.text = LootLockerTool.Instance.PlayerName;
                _playerLabel.alpha = 1f;
            }
        }
    }
}