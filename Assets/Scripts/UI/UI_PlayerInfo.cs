using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    public class UI_PlayerInfo : MonoBehaviour
    {
        [Required, ChildGameObjectsOnly]
        [SerializeField] private TMP_Text _playerLabel;
        [Required, ChildGameObjectsOnly]
        [SerializeField] private TMP_Text _rankValueLabel;
        [Required, ChildGameObjectsOnly]
        [SerializeField] private TMP_Text _scoreValueLabel;

        private void OnEnable()
        {
            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnPlayerDataUpdated.AddListener(UpdateUI);
            }

            UpdateUI();
        }

        private void OnDisable()
        {
            if (LootLockerTool.instanceExists)
            {
                LootLockerTool.Instance.OnPlayerDataUpdated.RemoveListener(UpdateUI);
            }
        }

        private void UpdateUI()
        {
            if (LootLockerTool.instanceExists)
            {
                // Name
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

                // Rank
                if (LootLockerTool.Instance.Rank <= 0 )
                {
                    _rankValueLabel.text = "None";
                }
                else
                {
                    _rankValueLabel.text = "#" + NumberFormatter.FormatNumberWithCommas(LootLockerTool.Instance.Rank);
                }

                // Score
                _scoreValueLabel.text = NumberFormatter.FormatNumberWithCommas(LootLockerTool.Instance.HighScore);
            }
            else
            {
                Debug.Log($"Lootlocker missing, no player data found.");
            }
        }
    }
}