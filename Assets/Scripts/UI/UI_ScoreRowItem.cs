using LootLocker.Requests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class UI_ScoreRowItem : MonoBehaviour
    {
        [SerializeField] private LootLockerLeaderboardMember _leaderboardMember;
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private TMP_Text _scoreLabel;

        public LootLockerLeaderboardMember Member => _leaderboardMember;
        public string Rank => _rankLabel.text;
        public string PlayerName => _playerName.text;
        public string Score => _scoreLabel.text;

        public void Initialize(LootLockerLeaderboardMember member)
        {
            string playerName = member.player.name;

            if (PlayerPrefs.HasKey(PrefKeys.IsInKidMode) && PlayerPrefs.GetInt(PrefKeys.IsInKidMode) != 0)
                playerName = BadWordFilter.ContainsBadWord(playerName) ? "Blobber" : playerName;

            _playerName.text = string.IsNullOrWhiteSpace(playerName) ? "Some blob" : playerName;

            _rankLabel.text = NumberFormatter.FormatNumberWithCommas(member.rank);
            _scoreLabel.text = NumberFormatter.FormatNumberWithCommas(member.score);
        }

        public void Initialize(string rank, string playerName, int score)
        {
            _rankLabel.text = rank;
            _playerName.text = playerName;
            _scoreLabel.text = score < 0 ? string.Empty : NumberFormatter.FormatNumberWithCommas(score);
        }

        public void SetSelected()
        {
            _background.enabled = true;
        }

        public void SetNormal()
        {
            _background.enabled = false;
        }
    }
}